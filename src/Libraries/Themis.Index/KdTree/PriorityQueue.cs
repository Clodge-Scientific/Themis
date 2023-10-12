using Themis.Index.KdTree.Interfaces;

namespace Themis.Index.KdTree;

internal struct PriorityItem<TItem, TPriority>
{
    public TItem Item;
    public TPriority Priority;
}

public class PriorityQueue<TItem, TPriority> : IPriorityQueue<TItem, TPriority>
{
    public const int DefaultCapacity = 4;
    public const int Up = 1;
    public const int Down = -1;

    public int Capacity { get; private set; }
    public int Count { get; private set; }
    public ITypeMath<TPriority> PriorityMath { get; }

    PriorityItem<TItem, TPriority>[] queue;

    public PriorityQueue(ITypeMath<TPriority> priorityMath, int capacity = DefaultCapacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero!");

        this.Capacity = capacity;
        this.PriorityMath = priorityMath;

        queue = new PriorityItem<TItem, TPriority>[Capacity];
    }

    #region Private Methods
    private void ExpandCapacity()
    {
        //< Double the current capacity
        this.Capacity *= 2;

        //< Generate a new queue and copy the old into the start of the new one
        var newQ = new PriorityItem<TItem, TPriority>[Capacity];
        Array.Copy(queue, newQ, queue.Length);

        //< Replace the existing queue with the new one
        queue = newQ;
    }

    private void ReorderItem(int index, int offset)
    {
        if (offset != Up && offset != Down) throw new ArgumentException($"Offset must be 1 or -1, received: {offset}", nameof(offset));
        if (index < 0 || index >= Count) throw new IndexOutOfRangeException($"Index must be within [0, Count - 1]");

        var item = queue[index];
        int nextIndex = index + offset;

        while (nextIndex >= 0 && nextIndex < Count)
        {
            var next = queue[nextIndex];
            int comp = PriorityMath.Compare(item.Priority, next.Priority);

            //< If we're going up and our priority is higher than the next, swap
            //< If we're going down and our priority is lower than the next, swap
            if (offset == Down ? comp > 0 : comp < 0)
            {
                queue[index] = next;
                queue[nextIndex] = item;

                index += offset;
                nextIndex += offset;
            }
            else { break; }
        }
    }
    #endregion

    #region IPriorityQueue Methods
    public void Enqueue(TItem item, TPriority priority)
    {
        if (++Count > Capacity) ExpandCapacity();

        int idx = Count - 1;
        queue[idx] = new PriorityItem<TItem, TPriority> { Item = item, Priority = priority };

        ReorderItem(idx, Down);
    }

    public TItem Dequeue()
    {
        TItem item = queue[0].Item;

        queue[0].Item = default; //< Should handle this possible null ref, buuut.. nah
        queue[0].Priority = PriorityMath.MinValue;

        ReorderItem(0, Up);
        Count--;

        return item;
    }

    public TItem GetHighest()
    {
        if (Count == 0) throw new Exception("PriorityQueue is empty!");

        return queue[0].Item;
    }

    public TPriority GetHighestPriority()
    {
        if (Count == 0) throw new Exception("PriorityQueue is empty!");

        return queue[0].Priority;
    }
    #endregion
}