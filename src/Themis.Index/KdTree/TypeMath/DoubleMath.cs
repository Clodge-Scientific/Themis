namespace Themis.Index.KdTree.TypeMath;

public class DoubleMath : TypeMath<double>
{
    public override double Zero => 0.0;
    public override double MinValue => double.MinValue;
    public override double MaxValue => double.MaxValue;
    public override double NegativeInfinity => double.NegativeInfinity;
    public override double PositiveInfinity => double.PositiveInfinity;

    #region ITypeMath<T> Methods
    public override int Compare(double a, double b) => a.CompareTo(b);

    public override bool AreEqual(double a, double b) => a.Equals(b);

    public override double Add(double a, double b) => a + b;
    public override double Subtract(double a, double b) => a - b;
    public override double Multiply(double a, double b) => a * b;

    public override double DistanceSquaredBetweenPoints(IEnumerable<double> a, IEnumerable<double> b)
    {
        if (a.Count() != b.Count()) throw new ArgumentException($"Input IEnumerables must have same dimensionality - {a.Count()} != {b.Count()}");

        double dist = Zero;
        int dimensions = a.Count();

        foreach (int dim in Enumerable.Range(0, dimensions))
        {
            double distOnAxis = Subtract(a.ElementAt(dim), b.ElementAt(dim));
            double distOnAxisSquared = Multiply(distOnAxis, distOnAxis);

            dist += distOnAxisSquared;
        }

        return dist;
    }
    #endregion
}