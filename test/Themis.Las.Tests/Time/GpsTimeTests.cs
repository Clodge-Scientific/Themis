using System;

using Xunit;
using Assert = Xunit.Assert;

using Themis.Las.Time;

namespace Themis.Las.Tests.Time;

public class GpsTimeTests
{
    const int TestWeekNumber = 2195;
    const double TestWeekSeconds = 432018;

    static readonly DateTime TestDate = new(2022, 02, 04);
    static readonly DateTime TestWeekStartDate = new(2022, 01, 30);

    [Fact]
    public void GetWeekNoTest()
    {
        int ActualWeekNo = GpsTime.GetWeekNo(TestDate);

        Assert.Equal(TestWeekNumber, ActualWeekNo);
    }

    [Fact]
    public void ToGpsStandardTest()
    {
        double ActualGpsTime = GpsTime.ToGpsStandard(TestWeekNumber, TestWeekSeconds);
        DateTime ActualDate = GpsTime.Parse(ActualGpsTime);

        Assert.Equal(TestDate, ActualDate);
    }

    [Fact]
    public void ParseFromWeekAndSowTest()
    {
        double ExpectedGPStoUTC = -18.0;
        double ActualGPStoUTC = LeapSeconds.GPStoUTC(TestWeekStartDate);

        Assert.Equal(ExpectedGPStoUTC, ActualGPStoUTC);

        DateTime ExpectedDateUtc = TestWeekStartDate.AddSeconds(ActualGPStoUTC);
        DateTime ActualDateUtc = GpsTime.Parse(TestWeekNumber, 0.0);

        Assert.Equal(ExpectedDateUtc, ActualDateUtc);
    }

    [Fact]
    public void ParseFromGpsTimestampTest()
    {
        DateTime ActualDateUtc = GpsTime.Parse(TestWeekNumber, TestWeekSeconds);

        Assert.Equal(TestDate, ActualDateUtc);
    }
}