using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry
{
    public static class Extensions
    {
        /// <summary>
        /// Create a deep copy of an existing Vector and modify a single dimensional value (at a specified index)
        /// </summary>
        /// <param name="v">Existing vector to clone & modify</param>
        /// <param name="index">Dimensional index (0-based)</param>
        /// <param name="value">Expected value in newly created Vector for given dimensional index</param>
        /// <returns></returns>
        public static Vector<T> CloneModify<T>(this Vector<T> v, int index, T value)
            where T : struct, IFormattable, IEquatable<T>
        {
            var res = v.Clone();
            res[index] = value;
            return res;
        }

        /// <summary>
        /// Create a MathNet.Numerics.LinearAlgebra.Vector<typeparamref name="T"/> from any input IEnumerable<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Any input primitive type supported by Vector<typeparamref name="T"/></typeparam>
        /// <param name="values">Input IEnumerable<typeparamref name="T"/> to be transformed</param>
        /// <returns></returns>
        public static Vector<T> ToVector<T>(this IEnumerable<T> values)
            where T : struct, IFormattable, IEquatable<T>
        {
            return CreateVector.Dense(values.ToArray());
        }

        /// <summary>
        /// Compute the Vector Cross Product (A x B) of two (double-based) position Vectors 
        /// </summary>
        /// <param name="a">Position Vector A</param>
        /// <param name="b">Position Vector B</param>
        /// <returns>Resultant Vector C of (A x B)</returns>
        public static Vector<double> Cross(this Vector<double> a, Vector<double> b)
        {
            return CreateVector.Dense(new double[]
            {
                a[1] * b[2] - a[2] * b[1],
                a[2] * b[0] - a[0] * b[2],
                a[0] * b[1] - a[1] * b[0]
            });
        }
    }
}
