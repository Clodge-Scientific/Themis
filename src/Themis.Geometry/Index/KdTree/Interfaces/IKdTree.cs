using Themis.Geometry.Index.KdTree.TypeMath.Interfaces;

namespace Themis.Geometry.Index.KdTree.Interfaces
{
    public interface IKdTree<TKey, TValue> : IEnumerable<KdTreeNode<TKey, TValue>>
    {
        /// <summary>
        /// The total count of all items currently stored within the KdTree&lt;<typeparamref name="TKey"/>,<typeparamref name="TValue"/>&gt;
        /// </summary>
        int Count { get; }
        /// <summary>
        /// The assigned dimensionality (K) of the KdTree
        /// </summary>
        int Dimensions { get; }

        /// <summary>
        /// The assigned ITypeMath for the KdTree to compare dimensional values with each other
        /// </summary>
        public ITypeMath<TKey> TypeMath { get; }
        /// <summary>
        /// The assigned behaviour of the KdTree when attempting to add a HyperPoint whose position matches and existing HyperPoint
        /// </summary>
        public AddDuplicateBehavior AddDuplicateBehavior { get; }

        /// <summary>
        /// Add the input <typeparamref name="TValue"/> into the tree at the given IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(IEnumerable<TKey> point, TValue value);

        /// <summary>
        /// Remove the <typeparamref name="TValue"/> item stored at the input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint
        /// </summary>
        /// <param name="point">IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint to remove</param>
        void Remove(IEnumerable<TKey> point);
        /// <summary>
        /// Remove all elements currently stored within the tree
        /// </summary>
        void Clear();
        /// <summary>
        /// Attempts to re-create the KdTree into a balanced state
        /// </summary>
        void Balance();

        /// <summary>
        /// Search for the input <typeparamref name="TValue"/> within the tree and return the correspoding IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint if found
        /// </summary>
        /// <param name="value">Input <typeparamref name="TValue"/> search for</param>
        /// <param name="point">The IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint if found</param>
        /// <returns></returns>
        bool TryFindValue(TValue value, out IEnumerable<TKey> point);
        /// <summary>
        /// Attempt to find any <typeparamref name="TValue"/> currently stored within the tree at the input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint
        /// </summary>
        /// <param name="point">Input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint to check</param>
        /// <param name="value">The <typeparamref name="TValue"/> stored at that HyperPoint if found</param>
        /// <returns></returns>
        bool TryFindValueAt(IEnumerable<TKey> point, out TValue value);

        /// <summary>
        /// Search for the input <typeparamref name="TValue"/> within the tree
        /// </summary>
        /// <param name="value">Input <typeparamref name="TValue"/> to search for</param>
        /// <returns>IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint where the input <typeparamref name="TValue"/> is stored</returns>
        IEnumerable<TKey> FindValue(TValue value);
        /// <summary>
        /// Fetch the <typeparamref name="TValue"/> stored at the input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint
        /// </summary>
        /// <param name="point">The input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint to pull from</param>
        /// <returns>The <typeparamref name="TValue"/> stored at the input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint</returns>
        TValue FindValueAt(IEnumerable<TKey> point);

        /// <summary>
        /// Find and return all elements within the specified search distance
        /// </summary>
        /// <param name="point">Input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint to search around</param>
        /// <param name="radius">Scalar distance around input HyerPoint to search</param>
        /// <returns></returns>
        IEnumerable<KdTreeNode<TKey, TValue>> RadialSearch(IEnumerable<TKey> point, TKey radius);
        /// <summary>
        /// Find and return all elements within the specified search distance up to a maximum count
        /// </summary>
        /// <param name="point">Input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint to search around</param>
        /// <param name="radius">Scalar distance around input HyerPoint to search</param>
        /// <param name="count">Maximum count of points to return</param>
        /// <returns></returns>
        IEnumerable<KdTreeNode<TKey, TValue>> RadialSearch(IEnumerable<TKey> point, TKey radius, int count);
        /// <summary>
        /// Find and return the nearest neighbouring elements up to a maximum count
        /// </summary>
        /// <param name="point">Input IEnumerable&lt;<typeparamref name="TKey"/>&gt; HyperPoint to search around</param>
        /// <param name="count">Maximum count of points to return</param>
        /// <returns></returns>
        IEnumerable<KdTreeNode<TKey, TValue>> GetNearestNeighbours(IEnumerable<TKey> point, int count = int.MaxValue);
    }
}
