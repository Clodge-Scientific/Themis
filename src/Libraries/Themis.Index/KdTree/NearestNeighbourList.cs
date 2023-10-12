using Themis.Index.KdTree.Interfaces;

namespace Themis.Index.KdTree;

public class NearestNeighbourList<TItem, TDistance> : INearestNeighbourList<TItem, TDistance>
{
    public int MaximumCapacity { get; }
    public ITypeMath<TDistance> DistanceMath { get; }

    private PriorityQueue<TItem, TDistance> queue;

    public int Count => queue.Count;
    public bool IsAtCapacity => Count == MaximumCapacity;

    public NearestNeighbourList(ITypeMath<TDistance> distMath)
    {
        this.MaximumCapacity = int.MaxValue;
        this.DistanceMath = distMath;

        queue = new PriorityQueue<TItem, TDistance>(distMath);
    }

    public NearestNeighbourList(ITypeMath<TDistance> distMath, int maxCapacity)
    {
        this.MaximumCapacity = maxCapacity;
        this.DistanceMath = distMath;

        queue = new PriorityQueue<TItem, TDistance>(distMath, MaximumCapacity);
    }

    #region INearestNeighbourList Methods
    public bool Add(TItem item, TDistance dist)
    {
        if (queue.Count >= MaximumCapacity)
        {
            return CheckIfWorthAdding(item, dist);
        }

        queue.Enqueue(item, dist);
        return true;
    }

    bool CheckIfWorthAdding(TItem item, TDistance dist)
    {
        /* If the distance of this TItem is less than the distance of the 'last' neighbour item .. 
         *  .. in our neighbour list, then pop that neighbour off and push this one on.
         * Otherwise, don't even bother with the item
         * * * */
        if (DistanceMath.Compare(dist, queue.GetHighestPriority()) < 0)
        {
            queue.Dequeue();
            queue.Enqueue(item, dist);
            return true;
        }

        return false;
    }

    public TItem GetFurthest()
    {
        if (Count == 0) throw new Exception("NearestNeighbourList is empty!");

        return queue.GetHighest();
    }

    public TDistance GetFurthestDistance()
    {
        if (Count == 0) throw new Exception("NearestNeighbourList is empty!");

        return queue.GetHighestPriority();
    }

    public TItem RemoveFurthest()
    {
        if (Count == 0) throw new Exception("NearestNeighbourList is empty!");

        return queue.Dequeue();
    }
    #endregion
}