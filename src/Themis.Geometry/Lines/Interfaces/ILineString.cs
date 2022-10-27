using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Lines;

public interface ILineString
{
    /// <summary>
    /// Collection of all (ordered) LineSegments that form this LineString
    /// </summary>
    IList<LineSegment> Segments { get; }
    /// <summary>
    /// Collection of all (ordered) Vertices that form this LineString
    /// </summary>
    IList<Vector<double>> Vertices { get; }

    /// <summary>
    /// The total 3D length of all contained LineSegments
    /// </summary>
    double Length3D { get; }
    /// <summary>
    /// The total 2D length of all contained LineSegments
    /// </summary>
    double Length2D { get; }

    /// <summary>
    /// Get the closest point on the LineString (considering all contained LineSegments) to the input point-of-interest (POI)
    /// </summary>
    /// <param name="poi">Input POI position vector</param>
    /// <returns>Position vector of nearest point along LineString</returns>
    Vector<double> GetClosestPoint(Vector<double> poi);
}