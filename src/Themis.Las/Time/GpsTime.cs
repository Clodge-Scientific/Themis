namespace Themis.Las.Time;

public static class GpsTime
{
    /// <summary>
    /// The total number of seconds in a single day (24 * 60 * 60 = 86,400)
    /// </summary>
    public const long SecondsInDay = 86400;
    /// <summary>
    /// The total number of seconds in a single week (7 * 24 * 60 * 60 = 604,800)
    /// </summary>
    public const long SecondsInWeek = 604800;

    /// <summary>
    /// January 6th, 1980 - the start of the 'GPS Epoch'
    /// </summary>
    public static readonly DateTime GpsEpochStart = new DateTime(1980, 1, 6);

    const long GpsStandardTimeAdjustmentSeconds = 1000 * 1000 * 1000;
    /// <summary>
    /// The total time adjustment (in seconds) between GPS Standard Time and Adjusted GPS Standard Time
    /// </summary>
    public static readonly TimeSpan GpsStandardTimeAdjustment = TimeSpan.FromSeconds(GpsStandardTimeAdjustmentSeconds);

    /// <summary>
    /// Calculates GPS week number from an input DateTime
    /// </summary>
    /// <param name="date">Input DateTime object</param>
    /// <returns></returns>
    public static int GetWeekNo(DateTime dt)
    {
        return (int)(dt - TimeSpan.FromDays((int)dt.DayOfWeek) - GpsEpochStart).TotalDays / 7;
    }

    /// <summary>
    /// Calculates the GPS Standard Timestamp from input week number and week seconds
    /// </summary>
    /// <param name="week">GPS Week Number</param>
    /// <param name="sow">Seconds of week -> [0, 604800]</param>
    /// <returns></returns>
    public static double ToGpsStandard(int week, double sow)
    {
        return (week * SecondsInWeek - GpsStandardTimeAdjustmentSeconds) + sow;
    }

    /// <summary>
    /// Converts input GPS Standard Time into a DateTime object (in UTC)
    /// </summary>
    /// <param name="gpsTimestamp">Source timestamp in GPS Standard Time</param>
    /// <returns>DateTime UTC</returns>
    public static DateTime Parse(double gpsTimestamp)
    {
        //< Parse the timespan (in seconds) relative to the GPS epoch start
        TimeSpan ts = TimeSpan.FromTicks((long)(gpsTimestamp * TimeSpan.TicksPerSecond));
        TimeSpan tsAdj = ts + GpsStandardTimeAdjustment;
        DateTime dt = GpsEpochStart.AddSeconds(tsAdj.TotalSeconds);

        //< Get the GPS-UTC offset (Leap Seconds)
        double ls = LeapSeconds.GPStoUTC(dt);

        //< Add the leap second offset and return the DateTime
        DateTime result = dt.AddSeconds(ls);
        return result;
    }

    /// <summary>
    /// Converts the input GPS Week Number and GPS Seconds of Week into a DateTime object (in UTC)
    /// </summary>
    /// <param name="week"></param>
    /// <param name="sow"></param>
    /// <returns>DateTime UTC</returns>
    public static DateTime Parse(int week, double sow)
    {
        double secs = week * SecondsInWeek + sow;
        DateTime dt = GpsEpochStart.AddSeconds(secs);

        //< Get the GPS-UTC offset (Leap Seconds)
        double ls = LeapSeconds.GPStoUTC(dt);

        //< Add the leap second offset and return the DateTime
        DateTime result = dt.AddSeconds(ls);
        return result;
    }
}