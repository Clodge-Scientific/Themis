using Themis.Geometry.Boundary.Interfaces;

namespace Themis.Index.QuadTree;

internal interface IQuadTreeNode<T>
        where T : class
{
    /// <summary>
    /// The total count of all <typeparamref name="T"/> Items contained within the QuadTreeNode
    /// </summary>
    int Count { get; }
    /// <summary>
    /// The maximum number of <typeparamref name="T"/> Items allowed within a single QuadTreeNode
    /// </summary>
    int MaxItems { get; }

    /// <summary>
    /// The X-Coordinate of the QuadTreeNode's centroid
    /// </summary>
    double CentroidX { get; }
    /// <summary>
    /// The X-Coordinate of the QuadTreeNode's centroid
    /// </summary>
    double CentroidY { get; }

    /// <summary>
    /// The 2D IBoundingBox envelope of all <typeparamref name="T"/> Items within the QuadTreeNode
    /// </summary>
    IBoundingBox Envelope { get; }

    /// <summary>
    /// All <typeparamref name="T"/> Items contained within the QuadTreeNode and all child QuadTreeNodes
    /// </summary>
    IEnumerable<T> Items { get; }
    /// <summary>
    /// All <typeparamref name="T"/> Items and their IBoundingBox envelopes within the QuadTreeNode and all child QuadTreeNodes
    /// </summary>
    IEnumerable<KeyValuePair<T, IBoundingBox>> ItemsWithBounds { get; }

    /// <summary>
    /// Add a new <typeparamref name="T"/> Item to the QuadTreeNode with the specified 2D minima and maxima
    /// </summary>
    /// <param name="item"><typeparamref name="T"/> Item to be added</param>
    /// <param name="minX">Minimum X-Coordinate of the Item's 2D BoundingBox</param>
    /// <param name="minY">Minimum Y-Coordinate of the Item's 2D BoundingBox</param>
    /// <param name="maxX">Maximum X-Coordinate of the Item's 2D BoundingBox</param>
    /// <param name="maxY">Maximum Y-Coordinate of the Item's 2D BoundingBox</param>
    void Add(T item, double minX, double minY, double maxX, double maxY);
    /// <summary>
    /// Add a new <typeparamref name="T"/> Item to the QuadTreeNode with the specified IBoundingBox envelope
    /// </summary>
    /// <param name="item"><typeparamref name="T"/> Item to be added</param>
    /// <param name="bb">IBoundingBox envelope - will be reduced to 2D</param>
    void Add(T item, IBoundingBox bb);

    /// <summary>
    /// Remove an existing <typeparamref name="T"/> Item from the QuadTreeNode with the specified 2D minima and maxima
    /// </summary>
    /// <param name="item"></param>
    /// <param name="minX"></param>
    /// <param name="minY"></param>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    void Remove(T item, double minX, double minY, double maxX, double maxY);
    /// <summary>
    /// Remove an existing <typeparamref name="T"/> Item from the QuadTreeNode with the specified IBoundingBox envelope
    /// </summary>
    /// <param name="item"></param>
    /// <param name="bb"></param>
    void Remove(T item, IBoundingBox bb);

    /// <summary>
    /// Query and return all <typeparamref name="T"/> Items that intersect with the input (X, Y) POI
    /// </summary>
    /// <param name="x">X-Coordinate of query POI</param>
    /// <param name="y">Y-Coordinate of query POI</param>
    /// <returns></returns>
    IEnumerable<T> Query(double x, double y);
    /// <summary>
    /// Query and return all <typeparamref name="T"/> Items that intersect with (or are contained by) the input IBoundingBox query region
    /// </summary>
    /// <param name="bb">IBoundingBox query region</param>
    /// <returns></returns>
    IEnumerable<T> Query(IBoundingBox bb);

    /// <summary>
    /// Get the quadrant index for the given 2D (X, Y) POI
    /// </summary>
    /// <param name="x">X-Coordinate of POI</param>
    /// <param name="y">Y-Coordinate of POI</param>
    /// <returns></returns>
    int GetIndex(double x, double y);
}