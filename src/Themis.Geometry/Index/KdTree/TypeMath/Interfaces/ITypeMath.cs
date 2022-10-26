namespace Themis.Geometry.Index.KdTree.TypeMath.Interfaces
{
    public interface ITypeMath<T>
    {
        /// <summary>
        /// Return the representation of Zero for the given Type <typeparamref name="T"/>
        /// </summary>
        T Zero { get; }

        /// <summary>
        /// Return the minimum value possible for the given Type <typeparamref name="T"/>
        /// </summary>
        T MinValue { get; }
        /// <summary>
        /// Return the maximum value possible for the given Type <typeparamref name="T"/>
        /// </summary>
        T MaxValue { get; }

        /// <summary>
        /// Return the representation of Negative Infinity for the given Type <typeparamref name="T"/>
        /// </summary>
        T NegativeInfinity { get; }
        /// <summary>
        /// Return the representation of Positive Infinity for the given Type <typeparamref name="T"/>
        /// </summary>
        T PositiveInfinity { get; }

        /// <summary>
        /// Return the boolean result of the requisite equality comparison for Type <typeparamref name="T"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool AreEqual(T a, T b);
        /// <summary>
        /// Return the boolean result of the requisite sequence equality comparison for an IEnumerable&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        /// <param name="a">First IEnumerable&lt;<typeparamref name="T"/>&gt;</param>
        /// <param name="b">Second IEnumerable&lt;<typeparamref name="T"/>&gt;</param>
        /// <returns></returns>
        bool AreEqual(IEnumerable<T> a, IEnumerable<T> b);

        /// <summary>
        /// Comparison function that returns an integer value based on result (0 for equality, positive/negative integers otherwise)
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> value to compare</param>
        /// <param name="b">Second <typeparamref name="T"/> value to compare</param>
        /// <returns></returns>
        int Compare(T a, T b);

        /// <summary>
        /// Compare and return the smaller of the two input <typeparamref name="T"/> values
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> value to compare</param>
        /// <param name="b">Second <typeparamref name="T"/> value to compare</param>
        /// <returns>The 'smaller' of the two input <typeparamref name="T"/></returns>
        T Min(T a, T b);
        /// <summary>
        /// Compare and return the larger of the two input <typeparamref name="T"/> values
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> value to compare</param>
        /// <param name="b">Second <typeparamref name="T"/> value to compare</param>
        /// <returns>The 'larger' of the two input <typeparamref name="T"/></returns>
        T Max(T a, T b);
        /// <summary>
        /// Add the second input <typeparamref name="T"/> (b) to the first <typeparamref name="T"/> (a)
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> value to add</param>
        /// <param name="b">Second <typeparamref name="T"/> value to add</param>
        /// <returns></returns>
        T Add(T a, T b);
        /// <summary>
        /// Subtract the second input <typeparamref name="T"/> (b) from the first <typeparamref name="T"/> (a)
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> value</param>
        /// <param name="b">Second <typeparamref name="T"/> value</param>
        /// <returns></returns>
        T Subtract(T a, T b);
        /// <summary>
        /// Multiply the first input <typeparamref name="T"/> (a) by the second input <typeparamref name="T"/> (b)
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> value</param>
        /// <param name="b">Second <typeparamref name="T"/> value</param>
        /// <returns></returns>
        T Multiply(T a, T b);
        /// <summary>
        /// Generate the absolute distance between the two input IEnumerable&lt;<typeparamref name="T"/>&gt; that represent K-dimensional hyper points
        /// </summary>
        /// <param name="a">First <typeparamref name="T"/> position vector</param>
        /// <param name="b">Second <typeparamref name="T"/> position vector</param>
        /// <returns></returns>
        T DistanceSquaredBetweenPoints(IEnumerable<T> a, IEnumerable<T> b);
    }
}
