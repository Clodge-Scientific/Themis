using Themis.Geometry.Index.KdTree.TypeMath.Interfaces;

namespace Themis.Geometry.Index.KdTree.TypeMath
{
    public abstract class TypeMath<T> : ITypeMath<T>
    {
        #region ITypeMath<T> Properties
        public abstract T Zero { get; }
        public abstract T MinValue { get; }
        public abstract T MaxValue { get; }
        public abstract T NegativeInfinity { get; }
        public abstract T PositiveInfinity { get; }
        #endregion

        #region ITypeMath<T> Methods
        public abstract int Compare(T a, T b);
        public abstract bool AreEqual(T a, T b);

        public abstract T Add(T a, T b);
        public abstract T Subtract(T a, T b);
        public abstract T Multiply(T a, T b);
        public abstract T DistanceSquaredBetweenPoints(IEnumerable<T> a, IEnumerable<T> b);

        public virtual bool AreEqual(IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a.Count() != b.Count()) return false;

            foreach (int i in Enumerable.Range(0, a.Count()))
            {
                if (!AreEqual(a.ElementAt(i), b.ElementAt(i))) return false;
            }

            return true;
        }

        public T Min(T a, T b)
        {
            return Compare(a, b) < 0 ? a : b;
        }

        public T Max(T a, T b)
        {
            return Compare(a, b) > 0 ? a : b;
        }
        #endregion
    }
}
