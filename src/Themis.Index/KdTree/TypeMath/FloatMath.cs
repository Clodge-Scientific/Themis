namespace Themis.Index.KdTree.TypeMath;

public class FloatMath : TypeMath<float>
{
    public override float Zero => 0.0f;
    public override float MinValue => float.MinValue;
    public override float MaxValue => float.MaxValue;
    public override float NegativeInfinity => float.NegativeInfinity;
    public override float PositiveInfinity => float.PositiveInfinity;

    #region ITypeMath<T> Methods
    public override int Compare(float a, float b) => a.CompareTo(b);

    public override bool AreEqual(float a, float b) => a.Equals(b);

    public override float Add(float a, float b) => a + b;
    public override float Subtract(float a, float b) => a - b;
    public override float Multiply(float a, float b) => a * b;

    public override float DistanceSquaredBetweenPoints(IEnumerable<float> a, IEnumerable<float> b)
    {
        if (a.Count() != b.Count()) throw new ArgumentException($"Input IEnumerables must have same dimensionality - {a.Count()} != {b.Count()}");

        float dist = Zero;
        int dimensions = a.Count();

        foreach (int dim in Enumerable.Range(0, dimensions))
        {
            float distOnAxis = Subtract(a.ElementAt(dim), b.ElementAt(dim));
            float distOnAxisSquared = Multiply(distOnAxis, distOnAxis);

            dist += distOnAxisSquared;
        }

        return dist;
    }
    #endregion
}