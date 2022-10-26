using Themis.Geometry.Lines.Interfaces;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Lines
{
    public class LineString : ILineString, IEquatable<LineString>
    {
        public const int MinimumVertices = 2;

        public IList<LineSegment> Segments { get; protected set; }
        public IList<Vector<double>> Vertices { get; protected set; }

        public double Length3D => Segments.Sum(s => s.Length);
        public double Length2D => Segments.Sum(s => s.Length2D);

        public LineString(IEnumerable<Vector<double>> vertices)
        {
            if (vertices.Count() < MinimumVertices) throw new ArgumentException($"Cannot generate LineString from less than {MinimumVertices} points. Was given: {vertices.Count()}");

            this.Vertices = vertices.ToList();
            this.Segments = GenerateSegments(Vertices);
        }

        #region Constructor Helpers
        static IList<LineSegment> GenerateSegments(IList<Vector<double>> vertices)
        {
            return Enumerable.Range(0, vertices.Count() - 1)
                             .Select(i => new LineSegment(vertices[i], vertices[i + 1]))
                             .Where(s => s.Length > 0.0) //< Zero-length segments are from duplicated vertices, want to remove
                             .ToList();
        }
        #endregion

        public Vector<double> GetClosestPoint(Vector<double> poi)
        {
            return Segments.Select(s => s.GetClosestPoint(poi))  //< Get the closest point to each LineSegment
                           .OrderBy(pnt => (pnt - poi).L2Norm()) //< Order closest points by distance to POI (in ascending order)
                           .First();                             //< Return the closest (first) point
        }

        #region IEquatable
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            return Equals(obj as LineString);
        }

        public bool Equals(LineString? other)
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
