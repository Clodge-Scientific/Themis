using System;

using Xunit;
using Assert = Xunit.Assert;

using Themis.Las.Time;

namespace Themis.Las.Tests.Time;

public class LeapSecondsTest
{
    const double TestGPStoUTC = -18.0;
    const double TestUTCtoGPS = 18.0;

    static readonly DateTime TestDate = new(2022, 02, 04);

    [Fact]
    public void GpsToUtcTest()
    {
        var ActualOffset = LeapSeconds.GPStoUTC(TestDate);

        Assert.Equal(TestGPStoUTC, ActualOffset);
    }

    [Fact]
    public void UtcToGpsTest()
    {
        var ActualOffset = LeapSeconds.UTCtoGPS(TestDate);

        Assert.Equal(TestUTCtoGPS, ActualOffset);
    }
}