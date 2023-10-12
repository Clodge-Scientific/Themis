using Themis.Index.KdTree.Interfaces;

namespace Themis.Index.KdTree;

public interface INearestNeighbourList<TItem, TDistance>
{
    /// <summary>
    /// Total count of all <typeparamref name="TItem"/> elements in the list
    /// </summary>
    int Count { get; }
    /// <summary>
    /// The maximum number of <typeparamref name="TItem"/> elements we can store in the list
    /// </summary>
    int MaximumCapacity { get; }

    /// <summary>
    /// Boolean flag indicating whether or not the list is full
    /// </summary>
    bool IsAtCapacity { get; }

    /// <summary>
    /// The assigned ITypeMath to be used to compare <typeparamref name="TDistance"/> values
    /// </summary>
    ITypeMath<TDistance> DistanceMath { get; }

    /// <summary>
    /// Attempt to add a given <typeparamref name="TItem"/> into the list
    /// </summary>
    /// <param name="item">Input <typeparamref name="TItem"/> item</param>
    /// <param name="dist">Corresponding <typeparamref name="TDistance"/> distance value</param>
    /// <returns></returns>
    bool Add(TItem item, TDistance dist);

    /// <summary>
    /// Retrieve the 'furthest' away <typeparamref name="TItem"/> element within the list
    /// </summary>
    /// <returns><typeparamref name="TItem"/> with the largest <typeparamref name="TDistance"/> value</returns>
    TItem GetFurthest();
    /// <summary>
    /// Retrieve the furthest (largest) <typeparamref name="TDistance"/> currently in the list
    /// </summary>
    /// <returns>The largest <typeparamref name="TDistance"/> within the list</returns>
    TDistance GetFurthestDistance();
    /// <summary>
    /// Remove the current furthest away <typeparamref name="TItem"/> element within the list and return it
    /// </summary>
    /// <returns><typeparamref name="TItem"/> with the largest <typeparamref name="TDistance"/> value</returns>
    TItem RemoveFurthest();
}