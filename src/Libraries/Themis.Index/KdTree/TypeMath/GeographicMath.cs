using Themis.Geometry;

namespace Themis.Index.KdTree.TypeMath;

public class GeographicMath : DoubleMath
{
    const double DEGREES_ARC_TO_KILOMETERS = 60.0 * 1.1515 * 1.609344;
    const double KILOMETERS_TO_METERS = 1000.0;

    public override double DistanceSquaredBetweenPoints(IEnumerable<double> a, IEnumerable<double> b)
    {
        if (a.Count() < 2) throw new ArgumentException($"Input geographic position must be (at least) 2D", nameof(a));
        if (b.Count() < 2) throw new ArgumentException($"Input geographic position must be (at least) 2D", nameof(b));

        double dist = DistanceBetweenMeters(a.ElementAt(0), a.ElementAt(1), b.ElementAt(0), b.ElementAt(1));
        return dist * dist;
    }

    /// <summary>
    /// Compute the distance (in meters) between two geographic coordinates
    /// </summary>
    /// <param name="lonA">Longitude (x) of the first coordinate</param>
    /// <param name="latA">Latitude (y) of the first coordinate</param>
    /// <param name="lonB">Longitude (x) of the second coordinate</param>
    /// <param name="latB">Latitude (y) of the second coordinate</param>
    /// <returns></returns>
    public static double DistanceBetweenMeters(double lonA, double latA, double lonB, double latB)
    {
        if (lonA == lonB && latA == latB) return 0.0;

        //< Get the total angle between points (difference in longitude)
        double theta = lonA - lonB;
        //< Then, grab the components of the arc length
        double A = Math.Sin(Functions.ToRadians(latA)) * Math.Sin(Functions.ToRadians(latB));
        double B = Math.Cos(Functions.ToRadians(latA)) * Math.Cos(Functions.ToRadians(latB)) * Math.Cos(Functions.ToRadians(theta));
        //< Compute the total distance in degrees of arc, then convert to miles
        double distKm = DEGREES_ARC_TO_KILOMETERS * Functions.ToDegrees(Math.Acos(A + B));

        return KILOMETERS_TO_METERS * distKm;
    }
}