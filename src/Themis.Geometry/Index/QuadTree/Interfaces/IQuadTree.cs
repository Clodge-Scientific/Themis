using Themis.Geometry.Boundary.Interfaces;

namespace Themis.Geometry.Index.QuadTree.Interfaces
{
    public interface IQuadTree<T>
        where T : class
    {
        /// <summary>
        /// The total count of <typeparamref name="T"/> items currently contained within the QuadTree index
        /// </summary>
        int Count { get; }

        /// <summary>
        /// All <typeparamref name="T"/> items currently contained within the QuadTree index
        /// </summary>
        T[] Items { get; }

        /// <summary>
        /// Add an item to the QuadTree<typeparamref name="T"/> with the specified IBoundingBox extents
        /// </summary>
        /// <param name="item"><typeparamref name="T"/> Item to be added</param>
        /// <param name="bb">IBoundingBox extents of the Item</param>
        void Add(T item, IBoundingBox bb);
        /// <summary>
        /// Add an item to the QuadTree<typeparamref name="T"/> with the specified 2D minima/maxima
        /// </summary>
        /// <param name="item"><typeparamref name="T"/> Item to be added</param>
        /// <param name="minX">Minimum X-Coordinate of the Item's BoundingBox</param>
        /// <param name="minY">Minimum Y-Coordinate of the Item's BoundingBox</param>
        /// <param name="maxX">Maximum X-Coordinate of the Item's BoundingBox</param>
        /// <param name="maxY">Maximum Y-Coordinate of the Item's BoundingBox</param>
        void Add(T item, double minX, double minY, double maxX, double maxY);

        /// <summary>
        /// Remove an item from the QuadTree<typeparamref name="T"/>
        /// </summary>
        /// <param name="item"></param>
        void Remove(T item);

        /// <summary>
        /// Get all Items whose IBoundingBox extents intersect with the specified POI (X, Y)
        /// NOTE: Will not ensure uniqueness of returned items - items present on multiple nodes can be returned many times
        /// </summary>
        /// <param name="x">X-Coordinate of the Point-of-Interest</param>
        /// <param name="y">Y-Coordinate of the Point-of-Interest</param>
        /// <returns></returns>
        IEnumerable<T> QueryNonDistinct(double x, double y);
        /// <summary>
        /// Get all Items whose IBoundingBox extents intersect with the specified IBoundingBox query region
        /// NOTE: Will not ensure uniqueness of returned items - items present on multiple nodes can be returned many times
        /// </summary>
        /// <param name="bb">IBoundingBox query region</param>
        /// <returns></returns>
        IEnumerable<T> QueryNonDistinct(IBoundingBox bb);

        /// <summary>
        /// Get all distinct Items whose IBoundingBox extents intersect with the specified POI (X, Y)
        /// </summary>
        /// <param name="x">X-Coordinate of Point-of-Interest</param>
        /// <param name="y">Y-Coordinate of Point-of-Interest</param>
        /// <returns></returns>
        IEnumerable<T> QueryDistinct(double x, double y);
        /// <summary>
        /// Get all distinct Items whose IBoundingBox extents intersect with the specified IBoundingBox query region
        /// </summary>
        /// <param name="bb">IBoundingBox query region</param>
        /// <returns></returns>
        IEnumerable<T> QueryDistinct(IBoundingBox bb);
    }
}
