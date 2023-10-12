namespace Themis.Geometry;

public static class Functions
{
    public const double DEG_TO_RAD = Math.PI / 180.0;
    public const double RAD_TO_DEG = 180.0 / Math.PI;

    /// <summary>
    /// Converts input decimal degrees to decimal radians
    /// </summary>
    /// <param name="angleDeg">Decimal Degrees to be converted</param>
    /// <returns>Decimal radians (as double)</returns>
    public static double ToRadians(double angleDeg)
    {
        return angleDeg * DEG_TO_RAD;
    }

    /// <summary>
    /// Converts input decimal radians to decimal degrees
    /// </summary>
    /// <param name="angleRad">Decimal Radians to be converted</param>
    /// <returns>Decimal degrees (as double</returns>
    public static double ToDegrees(double angleRad)
    {
        return angleRad * RAD_TO_DEG;
    }
}
