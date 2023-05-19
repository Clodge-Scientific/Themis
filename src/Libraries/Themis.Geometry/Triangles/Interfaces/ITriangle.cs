using Themis.Geometry.Lines;
using Themis.Geometry.Boundary;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Triangles;

public interface ITriangle
{
    /// <summary>
    /// Dot product of the Triangle's Normal vector onto the 'first' vertex
    /// </summary>
    double D { get; }
    /// <summary>
    /// The 3D Triangle geometry's Normal vector (orthogonal to Triangle surface)
    /// </summary>
    Vector<double> Normal { get; }

    /// <summary>
    /// The 2D BoundingBox envelope of the 2D projection of this Triangle geometry
    /// </summary>
    IBoundingBox Envelope { get; }

    /// <summary>
    /// Collection of the 3 LineSegments that compose this Triangle geometry
    /// </summary>
    IList<LineSegment> Edges { get; }
    /// <summary>
    /// Collection of the 3 Vertices that compose this Triangle geometry
    /// </summary>
    IList<Vector<double>> Vertices { get; }

    /// <summary>
    /// Check if the given position vector is contained by the 2D projection of this Triangle geometry
    /// </summary>
    /// <param name="pos">Position vector to be checked</param>
    /// <returns>True if contained within 2D projection, false otherwise</returns>
    bool Contains(Vector<double> pos);
    /// <summary>
    /// Check if the given (x, y) position is contained by the 2D projection of this Triangle geometry
    /// </summary>
    /// <param name="x">X-Coordinate of query position</param>
    /// <param name="y">Y-Coordinate of query position</param>
    /// <returns>True if contained within 2D projection, false otherwise</returns>
    bool Contains(double x, double y);

    /// <summary>
    /// Extract the elevation (Z) on the Triangle's surface for the given position vector
    /// </summary>
    /// <param name="pos">Position vector to drape onto Triangle surface</param>
    /// <returns>Triangle surface elevation at position</returns>
    double GetZ(Vector<double> pos);
    /// <summary>
    /// Extract the elevation (Z) on the Triangle's surface for the given (x, y) position
    /// </summary>
    /// <param name="x">X-Coordinate of query position</param>
    /// <param name="y">Y-Coordinate of query position</param>
    /// <returns>Triangle surface elevation at position</returns>
    double GetZ(double x, double y);
}