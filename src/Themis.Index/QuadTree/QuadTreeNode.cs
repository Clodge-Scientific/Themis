using Themis.Geometry.Boundary;
using Themis.Geometry.Boundary.Interfaces;

namespace Themis.Index.QuadTree;

internal class QuadTreeNode<T> : IQuadTreeNode<T>
        where T : class
{
    public int Count { get; private set; } = 0;

    public double CentroidX { get; }
    public double CentroidY { get; }

    public int MaxItems { get; }
    public IBoundingBox Envelope { get; }

    public IEnumerable<T> Items => GetAllItems();
    public IEnumerable<KeyValuePair<T, IBoundingBox>> ItemsWithBounds => GetAllItemsWithBounds();

    QuadTreeNode<T>[]? children = null;

    Dictionary<T, IBoundingBox> itemsSmall = new Dictionary<T, IBoundingBox>();
    Dictionary<T, IBoundingBox> itemsLarge = new Dictionary<T, IBoundingBox>();

    #region Constructors
    public QuadTreeNode(double centerX, double centerY, double halfWidth, int maxItems)
    {
        this.MaxItems = maxItems;
        this.CentroidX = centerX;
        this.CentroidY = centerY;
        this.Envelope = BoundingBox.FromPoint(centerX, centerY, halfWidth);
    }

    private QuadTreeNode(double minX, double minY, double maxX, double maxY, int maxItems)
    {
        if (minX > maxX) throw new ArgumentException($"minX ({minX}) cannot be greater than maxX ({maxX})!");
        if (minY > maxY) throw new ArgumentException($"minY ({minY}) cannot be greater than maxY ({maxY})!");

        this.MaxItems = maxItems;
        this.CentroidX = minX + (maxX - minX) / 2.0;
        this.CentroidY = minY + (maxY - minY) / 2.0;
        this.Envelope = BoundingBox.From(minX, minY, maxX, maxY);
    }
    #endregion

    public int GetIndex(double x, double y)
    {
        return (x < CentroidX ? 0 : 2) + (y < CentroidY ? 0 : 1);
    }

    #region IQuadTreeNode Add & Remove Methods
    public void Add(T item, double minX, double minY, double maxX, double maxY)
    {
        Add(item, BoundingBox.From(minX, minY, maxX, maxY));
    }

    public void Add(T item, IBoundingBox bb)
    {
        Count++;
        if (bb.Contains(CentroidX, CentroidY) && CheckCorners(bb))
        {
            itemsLarge.Add(item, bb);
        }
        else
        {
            if (children == null)
            {
                itemsSmall.Add(item, bb);
                if (itemsSmall.Count > MaxItems)
                {
                    Split();
                }
            }
            else
            {
                AddToChildren(item, bb);
            }
        }
    }

    bool CheckCorners(IBoundingBox bb)
    {
        return bb.Contains(Envelope.MinX, Envelope.MinY) || //< Lower-left corner
               bb.Contains(Envelope.MinX, Envelope.MaxY) || //< Upper-left corner
               bb.Contains(Envelope.MaxX, Envelope.MaxY) || //< Upper-right corner
               bb.Contains(Envelope.MaxX, Envelope.MinY) || //< Lower-right corner
                                                            //< This was added to deal with exceptionally narrow Envelopes - may need to add further corner case tests
               bb.Intersects(Envelope) && (bb.Width > Envelope.Width || bb.Height > Envelope.Height);
    }

    void AddToChildren(T item, IBoundingBox bb)
    {
        if (children == null) throw new ArgumentNullException("children");

        foreach (var child in children.Where(c => c.Envelope.Intersects(bb)))
        {
            child.Add(item, bb);
        }
    }

    public void Remove(T item, double minX, double minY, double maxX, double maxY)
    {
        Remove(item, BoundingBox.From(minX, minY, maxX, maxY));
    }

    public void Remove(T item, IBoundingBox bb)
    {
        Count--;
        if (itemsLarge.ContainsKey(item)) itemsLarge.Remove(item);

        if (children == null) itemsSmall.Remove(item);
        else
        {
            int childCount = 0;
            foreach (var child in children.Where(c => c.Envelope.Intersects(bb)))
            {
                child.Remove(item, bb);
                childCount += child.Count;
            }

            if (childCount == 0) Unsplit();
        }
    }
    #endregion

    #region IQuadTreeNode Item Retrieval Methods
    private IEnumerable<T> GetAllItems()
    {
        foreach (var item in itemsLarge) yield return item.Key;

        if (children == null)
        {
            foreach (var item in itemsSmall) yield return item.Key;
        }
        else
        {
            foreach (var item in children.SelectMany(c => c.Items)) yield return item;
        }
    }

    private IEnumerable<KeyValuePair<T, IBoundingBox>> GetAllItemsWithBounds()
    {
        foreach (var item in itemsLarge) yield return item;

        if (children == null)
        {
            foreach (var item in itemsSmall) yield return item;
        }
        else
        {
            foreach (var item in children.SelectMany(c => c.ItemsWithBounds)) yield return item;
        }
    }
    #endregion

    #region IQuadTreeNode Query Methods
    public IEnumerable<T> Query(double x, double y)
    {
        return Query(BoundingBox.FromPoint(x, y, BoundingBox.SinglePointBuffer));
    }

    public IEnumerable<T> Query(IBoundingBox bb)
    {
        foreach (var item in itemsLarge.Where(item => item.Value.Intersects(bb))) yield return item.Key;

        if (children == null)
        {
            foreach (var item in itemsSmall.Where(item => item.Value.Intersects(bb))) yield return item.Key;
        }
        else
        {
            foreach (var child in children.Where(c => c.Envelope.Intersects(bb)))
            {
                foreach (var item in child.Query(bb)) yield return item;
            }
        }
    }
    #endregion

    #region Split & Unsplit Methods
    public void Split()
    {
        //< Need to generate the four children QuadTreeNodes for the four sub-quadrants
        children = new[]
        {
                new QuadTreeNode<T>(Envelope.MinX, Envelope.MinY, CentroidX, CentroidY, MaxItems),
                new QuadTreeNode<T>(Envelope.MinX, CentroidY, CentroidX, Envelope.MaxY, MaxItems),
                new QuadTreeNode<T>(CentroidX, Envelope.MinY, Envelope.MaxX, CentroidY, MaxItems),
                new QuadTreeNode<T>(CentroidX, CentroidY, Envelope.MaxX, Envelope.MaxY, MaxItems),
            };

        //< Take all current items in 'itemsSmall' and push to the new child nodes
        foreach (var item in itemsSmall) AddToChildren(item.Key, item.Value);

        //< Clear out the 'itemsSmall' collection as we're no longer a leaf
        itemsSmall.Clear();
        //itemsSmall = null
    }

    public void Unsplit()
    {
        children = null;
        itemsSmall = new Dictionary<T, IBoundingBox>();
    }
    #endregion
}