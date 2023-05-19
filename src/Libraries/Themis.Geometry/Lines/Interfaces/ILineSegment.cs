using Themis.Geometry.Boundary;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Lines;

public interface ILineSegment
{
    /// <summary>
    /// Starting Vertex of the LineSegment
    /// </summary>
    Vector<double> A { get; }
    /// <summary>
    /// Terminating Vertex of the LineSegment
    /// </summary>
    Vector<double> B { get; }

    /// <summary>
    /// The Vector (Euclidean) magnitude of the current LineSegment formed by A->B
    /// </summary>
    double Length { get; }
    /// <summary>
    /// The 2D Vector (Euclidean) magnitude of the current LineSegment formed by A->B
    /// </summary>
    double Length2D { get; }

    /// <summary>
    /// Gets the Unit Vector pointing in the direction of the current LineSegment formed by A->B
    /// </summary>
    Vector<double> Unit { get; }

    /// <summary>
    /// The 2D BoundingBox envelope of the ILineSegment geometry
    /// </summary>
    IBoundingBox Envelope { get; }

    /// <summary>
    /// Get the projected magnitude (station) of the position vector v upon the infinite line formed by LineSegment A->B
    /// </summary>
    /// <param name="v">Input position vector</param>
    /// <returns>The scalar projected magnitude (station)</returns>
    double GetStation(Vector<double> v);
    /// <summary>
    /// Get the minimum distance between the input position vector 'v' and the LineSegment A->B
    /// </summary>
    /// <param name="v">Input position vector</param>
    /// <returns>The scalar distance between v and the closest point on A->B</returns>
    double DistanceToPoint(Vector<double> v);

    /// <summary>
    /// Extract the closest position along the LineSegment A->B to the given position vector 'v'
    /// NOTE:  Extracted points are limited to the discrete LineSegment A->B (station never exceeds [0, Length])
    /// </summary>
    /// <param name="v">Input position vector</param>
    /// <returns>Position vector of closest point on LineSegment A->B</returns>
    Vector<double> GetClosestPoint(Vector<double> v);
    /// <summary>
    /// Extract a point along the LineSegment A->B at the given input station (starting at A)
    /// </summary>
    /// <param name="station">Scalar station (relative to A) of desired result position</param>
    /// <returns>Position vector of point along A->B at input scalar station (T)</returns>
    Vector<double> ExtractPoint(double station);
}