using Themis.Geometry.Boundary;

namespace Themis.Geometry.Lines;

public class LineSegment : ILineSegment, IEquatable<LineSegment>
{
    public Vector<double> A { get; protected set; }
    public Vector<double> B { get; protected set; }

    public double Length => (B - A).L2Norm();
    public double Length2D => (B.CloneModify(2, 0) - A.CloneModify(2, 0)).L2Norm();

    public Vector<double> Unit => (B - A) / Length;

    public IBoundingBox Envelope => GenerateBoundingBox();

    /// <summary>
    /// Construct a new LineSegment deep-copying input vertices
    /// </summary>
    /// <param name="a">Starting Vertex (A) of the LineSegment</param>
    /// <param name="b">Terminating Vertex (B) of the LineSegment</param>
    public LineSegment(Vector<double> a, Vector<double> b)
    {
        this.A = a.Clone();
        this.B = b.Clone();
    }

    IBoundingBox GenerateBoundingBox()
    {
        var verts = new Vector<double>[] { A, B };

        double[] x = verts.Select(v => v[0]).ToArray();
        double[] y = verts.Select(v => v[1]).ToArray();

        return BoundingBox.From(x.Min(), y.Min(), x.Max(), y.Max());
    }

    public static LineSegment GenerateOrdered(ILineSegment line)
    {
        return GenerateOrdered(line.A, line.B);
    }

    public static LineSegment GenerateOrdered(Vector<double> a, Vector<double> b)
    {
        if (a[0] < b[0])
            return new LineSegment(a, b);
        else if (a[0] > b[0])
            return new LineSegment(b, a);
        else
        {
            //< X-coordinates are equal, order by Y
            if (a[1] < b[1]) return new LineSegment(a, b);
            
            return new LineSegment(b, a);
        }
    }

    #region ILineSegment Methods
    public double GetStation(Vector<double> v)
    {
        //< Get the vector A->V, then get magnitude projected onto our Unit vector
        return (v - A).DotProduct(Unit);
    }

    public double DistanceToPoint(Vector<double> v)
    {
        //< Get the closest point on the LineSegment - then return the distance from it to 'v'
        return (GetClosestPoint(v) - v).L2Norm();
    }

    public Vector<double> GetClosestPoint(Vector<double> v)
    {
        double station = GetStation(v);

        if (station <= 0) return A; //< If prior to Starting Vertex - just return A
        if (station >= Length) return B; //< If after the Terminating Vertex - just return B

        return ExtractPoint(station);
    }

    public Vector<double> ExtractPoint(double station)
    {
        return A + (station * Unit);
    }
    #endregion

    #region IEquatable
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        return Equals(obj as LineSegment);
    }

    public bool Equals(LineSegment? other)
    {
        return other != null &&
               A.SequenceEqual(other.A) &&
               B.SequenceEqual(other.B);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A.ToArray(), B.ToArray());
    }
    #endregion
}