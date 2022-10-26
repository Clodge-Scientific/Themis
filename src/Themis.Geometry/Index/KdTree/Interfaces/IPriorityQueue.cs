using Themis.Geometry.Index.KdTree.TypeMath.Interfaces;

namespace Themis.Geometry.Index.KdTree.Interfaces
{
    public interface IPriorityQueue<TItem, TPriority>
    {
        /// <summary>
        /// The total count of all <typeparamref name="TItem"/> elements within the IPriorityQueue
        /// </summary>
        int Count { get; }
        /// <summary>
        /// The current maximum capacity of the IPriorityQueue
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// The assigned ITypeMath to be used to compare <typeparamref name="TPriority"/> values
        /// </summary>
        ITypeMath<TPriority> PriorityMath { get; }

        /// <summary>
        /// Insert a new <typeparamref name="TItem"/> into the IPriorityQueue based on its given <typeparamref name="TPriority"/>
        /// </summary>
        /// <param name="item">Input <typeparamref name="TItem"/></param>
        /// <param name="priority">Input <typeparamref name="TItem"/>'s associated <typeparamref name="TPriority"/></param>
        void Enqueue(TItem item, TPriority priority);

        /// <summary>
        /// Remove and return the highest priority item from within the IPriorityQueue
        /// </summary>
        /// <returns><typeparamref name="TItem"/> with highest <typeparamref name="TPriority"/></returns>
        TItem Dequeue();
        /// <summary>
        /// Gets the highest priority item from within the IPriorityQueue
        /// </summary>
        /// <returns><typeparamref name="TItem"/> with highest <typeparamref name="TPriority"/></returns>
        TItem GetHighest();
        /// <summary>
        /// Gets the current highest <typeparamref name="TPriority"/> within the IPriorityQueue
        /// </summary>
        /// <returns></returns>
        TPriority GetHighestPriority();
    }
}
