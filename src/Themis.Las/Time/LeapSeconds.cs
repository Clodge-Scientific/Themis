namespace Themis.Las.Time;

public static class LeapSeconds
{
    /// <summary>
    /// All the dates (since Jan 6 1980) that a Leap Second has been added to the total offset
    /// TODO - Make this parse-able from app settings or something so it can be updated externally
    /// </summary>
    public static readonly DateTime[] LeapSecondDates = new DateTime[]
    {
            new DateTime(1981, 06, 30),
            new DateTime(1982, 06, 30),
            new DateTime(1983, 06, 30),
            new DateTime(1985, 06, 30),
            new DateTime(1987, 12, 31),
            new DateTime(1989, 12, 31),
            new DateTime(1990, 12, 31),
            new DateTime(1992, 06, 30),
            new DateTime(1993, 06, 30),
            new DateTime(1994, 06, 30),
            new DateTime(1995, 12, 31),
            new DateTime(1997, 06, 30),
            new DateTime(1998, 12, 31),
            new DateTime(2005, 12, 31),
            new DateTime(2008, 12, 31),
            new DateTime(2012, 06, 30),
            new DateTime(2015, 06, 30),
            new DateTime(2016, 12, 31),
    };

    /// <summary>
    /// Calculates the current total leap seconds between UTC and GPS time
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static double UTCtoGPS(DateTime dt)
    {
        var idx = ~Array.BinarySearch(LeapSecondDates, dt);
        return Math.Abs(idx);
    }

    /// <summary>
    /// Calculates the current total leap seconds between GPS and UTC time
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static double GPStoUTC(DateTime dt)
    {
        //< Must negate the result as we want to offset from GPS to UTC
        return -1.0 * UTCtoGPS(dt);
    }
}