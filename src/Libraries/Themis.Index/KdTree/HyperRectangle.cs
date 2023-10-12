using Themis.Index.KdTree.Interfaces;

namespace Themis.Index.KdTree;

public struct HyperRectangle<T>
{
    private T[] minimumPoint;
    private T[] maximumPoint;

    /// <summary>
    /// HyperPoint representing the minima of this HyperRectangle
    /// </summary>
    public T[] MinimumPoint
    {
        get { return minimumPoint; }
        set
        {
            minimumPoint = new T[value.Length];
            value.CopyTo(minimumPoint, 0);
        }
    }

    /// <summary>
    /// HyperPoint representing the maxima of this HyperRectangle
    /// </summary>
    public T[] MaximumPoint
    {
        get { return maximumPoint; }
        set
        {
            maximumPoint = new T[value.Length];
            value.CopyTo(maximumPoint, 0);
        }
    }

    /// <summary>
    /// Get the closest HyperPoint within the HyperRectangle&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <param name="point">Input HyperPoint</param>
    /// <param name="math">ITypeMath&lt;<typeparamref name="T"/>&gt; to use for value comparisons</param>
    /// <returns></returns>
    public T[] GetClosestPoint(T[] point, ITypeMath<T> math)
    {
        T[] closest = new T[point.Length];

        foreach (int dim in Enumerable.Range(0, point.Length))
        {
            if (math.Compare(minimumPoint[dim], point[dim]) > 0) { closest[dim] = minimumPoint[dim]; }      //< Smaller than minima - take minima
            else if (math.Compare(maximumPoint[dim], point[dim]) < 0) { closest[dim] = maximumPoint[dim]; } //< Larger than maxima - take maxima
            else { closest[dim] = point[dim]; }
        }

        return closest;
    }

    /// <summary>
    /// Generate a deep-copy of the current HyperRectangle&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <returns>Deep-copy of the current HyperRectangle&lt;<typeparamref name="T"/>&gt;</returns>
    public HyperRectangle<T> Clone()
    {
        return new HyperRectangle<T>
        {
            MinimumPoint = minimumPoint,
            MaximumPoint = maximumPoint
        };
    }

    /// <summary>
    /// Generate an infinite hyper-rectangle with the specified dimensionality and type
    /// </summary>
    /// <param name="dimensions">Number of dimensions for the resultant hyper-rectangle</param>
    /// <param name="math">ITypeMath&lt;<typeparamref name="T"/>&gt; to use for pos/neg infinity</param>
    /// <returns></returns>
    public static HyperRectangle<T> Infinite(int dimensions, ITypeMath<T> math)
    {
        var rect = new HyperRectangle<T>
        {
            MinimumPoint = new T[dimensions],
            MaximumPoint = new T[dimensions]
        };

        foreach (int dim in Enumerable.Range(0, dimensions))
        {
            rect.MinimumPoint[dim] = math.NegativeInfinity;
            rect.MaximumPoint[dim] = math.PositiveInfinity;
        }

        return rect;
    }
}