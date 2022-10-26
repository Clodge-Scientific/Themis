using Themis.Geometry.Boundary;
using Themis.Geometry.Boundary.Interfaces;

namespace Themis.Index.QuadTree;

public class QuadTree<T> : IQuadTree<T>
        where T : class
{
    public const int DefaultMaxItemsPerNode = 8;
    public int MaxItemsPerNode { get; }

    QuadTreeNode<T>? root = null;
    Dictionary<T, IBoundingBox> items = new Dictionary<T, IBoundingBox>();

    public int Count => root != null ? root.Count : 0;
    public T[] Items => items.Keys.ToArray();

    public bool IsRootNull => root == null;
    public IBoundingBox? RootEnvelope => (root != null) ? root.Envelope : null;

    public QuadTree(int maxItemsPerNode = DefaultMaxItemsPerNode)
    {
        this.MaxItemsPerNode = maxItemsPerNode;
    }

    public QuadTree(double minX, double minY, double maxX, double maxY, int maxItemsPerNode = DefaultMaxItemsPerNode)
        : this(maxItemsPerNode)
    {
        root = GetSquareNode(minX, minY, maxX, maxY, maxItemsPerNode);
    }

    public QuadTree(IBoundingBox bb, int maxItemsPerNode = DefaultMaxItemsPerNode)
        : this(maxItemsPerNode)
    {
        root = GetSquareNode(bb, maxItemsPerNode);
    }

    #region Static Methods
    static QuadTreeNode<T> GetSquareNode(IBoundingBox bb, int maxItemsPerNode)
    {
        return GetSquareNode(bb.MinX, bb.MinY, bb.MaxX, bb.MaxY, maxItemsPerNode);
    }

    static QuadTreeNode<T> GetSquareNode(double minX, double minY, double maxX, double maxY, int maxItemsPerNode)
    {
        double centerX = (maxX + minX) / 2.0;
        double centerY = (maxY + minY) / 2.0;
        double halfWidth = Math.Max(Math.Abs(maxX - minX), Math.Abs(maxY - minY)) / 2.0;

        return new QuadTreeNode<T>(centerX, centerY, halfWidth, maxItemsPerNode);
    }
    #endregion

    #region IQuadTree Add & Remove Methods
    public void Add(T item, double minX, double minY, double maxX, double maxY)
    {
        Add(item, BoundingBox.From(minX, minY, maxX, maxY));
    }

    public void Add(T item, IBoundingBox bb)
    {
        if (root == null) { root = GetSquareNode(bb, MaxItemsPerNode); }

        if (!RootNodeContains(bb)) { RecreateTreeToContain(bb); }

        root.Add(item, bb);
        items.Add(item, bb);
    }

    bool RootNodeContains(IBoundingBox bb)
    {
        if (root == null) return false;

        return root.Envelope.Contains(bb.MinX, bb.MinY) && root.Envelope.Contains(bb.MaxX, bb.MaxY);
    }

    void RecreateTreeToContain(IBoundingBox bb)
    {
        if (root == null) throw new ArgumentNullException($"Attempted to recreate tree with a null root QuadTreeNode.");

        //< Expand the IBoundingBox to include the input IBoundingBox
        var expandedEnv = root.Envelope.ExpandToInclude(bb);

        //< Take the max of the new BB's Width/Height - then buffer the new BB by that amount
        double bufferValue = Math.Max(expandedEnv.Width, expandedEnv.Height);
        var expandedEnvBuff = expandedEnv.Buffer(bufferValue);

        //< Generate the new root node from the expanded & buffer BB
        var newRoot = GetSquareNode(expandedEnvBuff, MaxItemsPerNode);

        foreach (var item in items) { newRoot.Add(item.Key, item.Value); }

        root = newRoot;
    }

    public void Remove(T item)
    {
        if (!items.ContainsKey(item)) return;
        if (root == null) return;

        var bb = items[item];
        if (bb == null) throw new ArgumentNullException($"Encountered null IBoundingBox envelope for Item: {item}");

        items.Remove(item);
        root.Remove(item, bb);
    }
    #endregion

    #region IQuadTree Query Methods
    public IEnumerable<T> QueryDistinct(double x, double y)
    {
        return QueryNonDistinct(x, y).Distinct();
    }

    public IEnumerable<T> QueryDistinct(IBoundingBox bb)
    {
        return QueryNonDistinct(bb).Distinct();
    }

    public IEnumerable<T> QueryNonDistinct(double x, double y)
    {
        return QueryNonDistinct(BoundingBox.FromPoint(x, y, BoundingBox.SinglePointBuffer));
    }

    public IEnumerable<T> QueryNonDistinct(IBoundingBox bb)
    {
        if (Count == 0 || root == null) return new T[] { };

        return root.Query(bb);
    }
    #endregion
}