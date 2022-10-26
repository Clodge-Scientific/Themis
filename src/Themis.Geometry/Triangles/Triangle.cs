using Themis.Geometry.Lines;
using Themis.Geometry.Boundary;
using Themis.Geometry.Triangles.Interfaces;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Triangles
{
    public class Triangle : ITriangle, IEquatable<Triangle>
    {
        /// <summary>
        /// Allowable error within containment calculations
        /// </summary>
        const double Epsilon = 1E-6;
        /// <summary>
        /// Total expected count of Triangle edges
        /// </summary>
        const int EdgeCount = 3;

        public double D => Normal.DotProduct(Vertices.First());
        public Vector<double> Normal => GetNormal(Vertices);

        public BoundingBox Envelope { get; protected set; }

        public IList<LineSegment> Edges { get; protected set; }
        public IList<Vector<double>> Vertices { get; protected set; }

        public Triangle(IEnumerable<Vector<double>> verts)
        {
            this.Vertices = verts.Select(v => v.Clone())
                                 .ToList();

            this.Envelope = GenerateBoundingBox(Vertices);
            this.Edges = GenerateEdges(Vertices);
        }

        #region Constructor Helpers
        static BoundingBox GenerateBoundingBox(IList<Vector<double>> verts)
        {
            double[] x = verts.Select(v => v[0]).ToArray();
            double[] y = verts.Select(v => v[1]).ToArray();

            return BoundingBox.From(x.Min(), y.Min(), x.Max(), y.Max());
        }

        static IList<LineSegment> GenerateEdges(IList<Vector<double>> verts)
        {
            return Enumerable.Range(0, EdgeCount)
                             .Select(i => new LineSegment(verts[i % EdgeCount], verts[(i + 1) % EdgeCount]))
                             .ToList();
        }

        static Vector<double> GetNormal(IList<Vector<double>> verts)
        {
            var A = verts[1] - verts[0];
            var B = verts[2] - verts[0];

            return A.Cross(B).Normalize(2);
        }
        #endregion

        #region ITriangle Methods
        public bool Contains(Vector<double> pos)
        {
            return Contains(pos[0], pos[1]);
        }

        public bool Contains(double x, double y)
        {
            double x1 = Vertices[0][0]; double y1 = Vertices[0][1];
            double x2 = Vertices[1][0]; double y2 = Vertices[1][1];
            double x3 = Vertices[2][0]; double y3 = Vertices[2][1];

            double denom = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
            double a = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / denom;
            double b = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / denom;
            double c = 1.0 - a - b;

            return IsWithin(a) && IsWithin(b) && IsWithin(c);
        }

        /// <summary>
        /// Checks if the input scalar coefficient is between [0, 1.0] with an allowed error of 1E-6
        /// </summary>
        /// <param name="value">Scalar coefficient to check</param>
        /// <returns>True if within [0 - 1E-6, 1.0 + 1E-6]</returns>
        static bool IsWithin(double value)
        {
            return (0 - Epsilon <= value && value <= 1 + Epsilon); 
        }

        public double GetZ(Vector<double> pos)
        {
            return GetZ(pos[0], pos[1]);
        }

        public double GetZ(double x, double y)
        {
            //< This will require some.. illustration as to how it magically works
            //< Otherwise this is some quake-tier inverse square hackery
            return (D - Normal[0] * x + Normal[1] * y) / Normal[2];
        }
        #endregion

        #region IEquatable
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            return Equals(obj as Triangle);
        }

        public bool Equals(Triangle? other)
        {
            return other != null &&
                   Vertices.SequenceEqual(other.Vertices);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Vertices.ToArray());
        }
        #endregion
    }
}
