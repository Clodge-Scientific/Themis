using Moq;
using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using Themis.Las.Structs;

namespace Themis.Las.Tests;

public class LasPointRecordFormatTests
{
    readonly Faker<LasPoint> _pointFaker;
    readonly Mock<ILasHeader> _headerMock;

    const int Dimensions = 3;
    const double MinPos = -1000.0;
    const double MaxPos = 1000.0;

    const double Scale = 0.01;
    const double Origin = 0.00;

    public LasPointRecordFormatTests()
    {
        #region Faker & Moq Instantiation
        _headerMock = new Mock<ILasHeader>();
        _headerMock.SetupGet(d => d.ScaleX).Returns(Scale);
        _headerMock.SetupGet(d => d.ScaleY).Returns(Scale);
        _headerMock.SetupGet(d => d.ScaleZ).Returns(Scale);
        _headerMock.SetupGet(d => d.OriginX).Returns(Origin);
        _headerMock.SetupGet(d => d.OriginY).Returns(Origin);
        _headerMock.SetupGet(d => d.OriginZ).Returns(Origin);

        _pointFaker = new Faker<LasPoint>()
            .RuleFor(lp => lp.UserData, f => f.Random.Byte())
            .RuleFor(lp => lp.Classification, f => f.Random.Byte())
            .RuleFor(lp => lp.Intensity, f => f.Random.UShort())
            .RuleFor(lp => lp.FlightLine, f => f.Random.UShort())
            .RuleFor(lp => lp.GlobalEncoding, f => f.Random.UShort())
            .RuleFor(lp => lp.ScanAngle, f => f.Random.Short())
            .RuleFor(lp => lp.Position, f => f.RandomPosition(Dimensions, MinPos, MaxPos))
            .RuleFor(lp => lp.Timestamp, f => f.Random.Double())
            .RuleFor(lp => lp.R, f => f.Random.UShort())
            .RuleFor(lp => lp.G, f => f.Random.UShort())
            .RuleFor(lp => lp.B, f => f.Random.UShort())
            .RuleFor(lp => lp.NIR, f => f.Random.UShort())
            .RuleFor(lp => lp.WavePacketDescriptorIndex, f => f.Random.Byte())
            .RuleFor(lp => lp.WaveformPacketSizeBytes, f => f.Random.UInt())
            .RuleFor(lp => lp.ReturnPointWaveformLocation, f => f.Random.Float())
            .RuleFor(lp => lp.X_t, f => f.Random.Float())
            .RuleFor(lp => lp.Y_t, f => f.Random.Float())
            .RuleFor(lp => lp.Z_t, f => f.Random.Float())
            .RuleFor(lp => lp.ByteOffsetToWaveformData, f => f.Random.ULong());
        #endregion
    }

    [Fact]
    public void GetLasPointRecordFormat0Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);
        int expectedGlobalEncoding = (byte)lpt.GlobalEncoding;
        int expectedScanAngle = (byte)lpt.ScanAngle;

        var lp0 = LasPointRecordFormat0.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp0.X);
        Assert.Equal(expectedY, lp0.Y);
        Assert.Equal(expectedZ, lp0.Z);
        Assert.Equal(lpt.Intensity, lp0.Intensity);
        Assert.Equal(expectedGlobalEncoding, lp0.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp0.Classification);
        Assert.Equal(expectedScanAngle, lp0.ScanAngle);
        Assert.Equal(lpt.UserData, lp0.UserData);
        Assert.Equal(lpt.FlightLine, lp0.FlightLine);
    }

    [Fact]
    public void GetLasPointRecordFormat1Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);
        int expectedGlobalEncoding = (byte)lpt.GlobalEncoding;
        int expectedScanAngle = (byte)lpt.ScanAngle;

        var lp1 = LasPointRecordFormat1.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp1.X);
        Assert.Equal(expectedY, lp1.Y);
        Assert.Equal(expectedZ, lp1.Z);
        Assert.Equal(lpt.Intensity, lp1.Intensity);
        Assert.Equal(expectedGlobalEncoding, lp1.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp1.Classification);
        Assert.Equal(expectedScanAngle, lp1.ScanAngle);
        Assert.Equal(lpt.UserData, lp1.UserData);
        Assert.Equal(lpt.FlightLine, lp1.FlightLine);
        Assert.Equal(lpt.Timestamp, lp1.Timestamp);
    }

    [Fact]
    public void GetLasPointRecordFormat2Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);
        int expectedGlobalEncoding = (byte)lpt.GlobalEncoding;
        int expectedScanAngle = (byte)lpt.ScanAngle;

        var lp2 = LasPointRecordFormat2.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp2.X);
        Assert.Equal(expectedY, lp2.Y);
        Assert.Equal(expectedZ, lp2.Z);
        Assert.Equal(lpt.Intensity, lp2.Intensity);
        Assert.Equal(expectedGlobalEncoding, lp2.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp2.Classification);
        Assert.Equal(expectedScanAngle, lp2.ScanAngle);
        Assert.Equal(lpt.UserData, lp2.UserData);
        Assert.Equal(lpt.FlightLine, lp2.FlightLine);
        Assert.Equal(lpt.R, lp2.R);
        Assert.Equal(lpt.G, lp2.G);
        Assert.Equal(lpt.B, lp2.B);
    }

    [Fact]
    public void GetLasPointRecordFormat3Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);
        int expectedGlobalEncoding = (byte)lpt.GlobalEncoding;
        int expectedScanAngle = (byte)lpt.ScanAngle;

        var lp3 = LasPointRecordFormat3.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp3.X);
        Assert.Equal(expectedY, lp3.Y);
        Assert.Equal(expectedZ, lp3.Z);
        Assert.Equal(lpt.Intensity, lp3.Intensity);
        Assert.Equal(expectedGlobalEncoding, lp3.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp3.Classification);
        Assert.Equal(expectedScanAngle, lp3.ScanAngle);
        Assert.Equal(lpt.UserData, lp3.UserData);
        Assert.Equal(lpt.FlightLine, lp3.FlightLine);
        Assert.Equal(lpt.Timestamp, lp3.Timestamp);
        Assert.Equal(lpt.R, lp3.R);
        Assert.Equal(lpt.G, lp3.G);
        Assert.Equal(lpt.B, lp3.B);
    }

    [Fact]
    public void GetLasPointRecordFormat4Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);
        int expectedGlobalEncoding = (byte)lpt.GlobalEncoding;
        int expectedScanAngle = (byte)lpt.ScanAngle;

        var lp4 = LasPointRecordFormat4.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp4.X);
        Assert.Equal(expectedY, lp4.Y);
        Assert.Equal(expectedZ, lp4.Z);
        Assert.Equal(lpt.Intensity, lp4.Intensity);
        Assert.Equal(expectedGlobalEncoding, lp4.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp4.Classification);
        Assert.Equal(expectedScanAngle, lp4.ScanAngle);
        Assert.Equal(lpt.UserData, lp4.UserData);
        Assert.Equal(lpt.FlightLine, lp4.FlightLine);
        Assert.Equal(lpt.Timestamp, lp4.Timestamp);
        Assert.Equal(lpt.WavePacketDescriptorIndex, lp4.WavePacketDescriptorIndex);
        Assert.Equal(lpt.ByteOffsetToWaveformData, lp4.ByteOffsetToWaveformData);
        Assert.Equal(lpt.WaveformPacketSizeBytes, lp4.WaveformPacketSizeBytes);
        Assert.Equal(lpt.ReturnPointWaveformLocation, lp4.ReturnPointWaveformLocation);
        Assert.Equal(lpt.X_t, lp4.X_t);
        Assert.Equal(lpt.Y_t, lp4.Y_t);
        Assert.Equal(lpt.Z_t, lp4.Z_t);
    }

    [Fact]
    public void GetLasPointRecordFormat5Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);
        int expectedGlobalEncoding = (byte)lpt.GlobalEncoding;
        int expectedScanAngle = (byte)lpt.ScanAngle;

        var lp5 = LasPointRecordFormat5.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp5.X);
        Assert.Equal(expectedY, lp5.Y);
        Assert.Equal(expectedZ, lp5.Z);
        Assert.Equal(lpt.Intensity, lp5.Intensity);
        Assert.Equal(expectedGlobalEncoding, lp5.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp5.Classification);
        Assert.Equal(expectedScanAngle, lp5.ScanAngle);
        Assert.Equal(lpt.UserData, lp5.UserData);
        Assert.Equal(lpt.FlightLine, lp5.FlightLine);
        Assert.Equal(lpt.Timestamp, lp5.Timestamp);
        Assert.Equal(lpt.R, lp5.R);
        Assert.Equal(lpt.G, lp5.G);
        Assert.Equal(lpt.B, lp5.B);
        Assert.Equal(lpt.WavePacketDescriptorIndex, lp5.WavePacketDescriptorIndex);
        Assert.Equal(lpt.ByteOffsetToWaveformData, lp5.ByteOffsetToWaveformData);
        Assert.Equal(lpt.WaveformPacketSizeBytes, lp5.WaveformPacketSizeBytes);
        Assert.Equal(lpt.ReturnPointWaveformLocation, lp5.ReturnPointWaveformLocation);
        Assert.Equal(lpt.X_t, lp5.X_t);
        Assert.Equal(lpt.Y_t, lp5.Y_t);
        Assert.Equal(lpt.Z_t, lp5.Z_t);
    }

    [Fact]
    public void GetLasPointRecordFormat6Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);

        var lp6 = LasPointRecordFormat6.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp6.X);
        Assert.Equal(expectedY, lp6.Y);
        Assert.Equal(expectedZ, lp6.Z);
        Assert.Equal(lpt.Intensity, lp6.Intensity);
        Assert.Equal(lpt.GlobalEncoding, lp6.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp6.Classification);
        Assert.Equal(lpt.ScanAngle, lp6.ScanAngle);
        Assert.Equal(lpt.UserData, lp6.UserData);
        Assert.Equal(lpt.FlightLine, lp6.FlightLine);
        Assert.Equal(lpt.Timestamp, lp6.Timestamp);
    }

    [Fact]
    public void GetLasPointRecordFormat7Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);

        var lp7 = LasPointRecordFormat7.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp7.X);
        Assert.Equal(expectedY, lp7.Y);
        Assert.Equal(expectedZ, lp7.Z);
        Assert.Equal(lpt.Intensity, lp7.Intensity);
        Assert.Equal(lpt.GlobalEncoding, lp7.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp7.Classification);
        Assert.Equal(lpt.ScanAngle, lp7.ScanAngle);
        Assert.Equal(lpt.UserData, lp7.UserData);
        Assert.Equal(lpt.FlightLine, lp7.FlightLine);
        Assert.Equal(lpt.Timestamp, lp7.Timestamp);
        Assert.Equal(lpt.R, lp7.R);
        Assert.Equal(lpt.G, lp7.G);
        Assert.Equal(lpt.B, lp7.B);
    }

    [Fact]
    public void GetLasPointRecordFormat8Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);

        var lp8 = LasPointRecordFormat8.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp8.X);
        Assert.Equal(expectedY, lp8.Y);
        Assert.Equal(expectedZ, lp8.Z);
        Assert.Equal(lpt.Intensity, lp8.Intensity);
        Assert.Equal(lpt.GlobalEncoding, lp8.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp8.Classification);
        Assert.Equal(lpt.ScanAngle, lp8.ScanAngle);
        Assert.Equal(lpt.UserData, lp8.UserData);
        Assert.Equal(lpt.FlightLine, lp8.FlightLine);
        Assert.Equal(lpt.Timestamp, lp8.Timestamp);
        Assert.Equal(lpt.R, lp8.R);
        Assert.Equal(lpt.G, lp8.G);
        Assert.Equal(lpt.B, lp8.B);
        Assert.Equal(lpt.NIR, lp8.NIR);
    }

    [Fact]
    public void GetLasPointRecordFormat9Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);

        var lp9 = LasPointRecordFormat9.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp9.X);
        Assert.Equal(expectedY, lp9.Y);
        Assert.Equal(expectedZ, lp9.Z);
        Assert.Equal(lpt.Intensity, lp9.Intensity);
        Assert.Equal(lpt.GlobalEncoding, lp9.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp9.Classification);
        Assert.Equal(lpt.ScanAngle, lp9.ScanAngle);
        Assert.Equal(lpt.UserData, lp9.UserData);
        Assert.Equal(lpt.FlightLine, lp9.FlightLine);
        Assert.Equal(lpt.Timestamp, lp9.Timestamp);
        Assert.Equal(lpt.WavePacketDescriptorIndex, lp9.WavePacketDescriptorIndex);
        Assert.Equal(lpt.ByteOffsetToWaveformData, lp9.ByteOffsetToWaveformData);
        Assert.Equal(lpt.WaveformPacketSizeBytes, lp9.WaveformPacketSizeBytes);
        Assert.Equal(lpt.ReturnPointWaveformLocation, lp9.ReturnPointWaveformLocation);
        Assert.Equal(lpt.X_t, lp9.X_t);
        Assert.Equal(lpt.Y_t, lp9.Y_t);
        Assert.Equal(lpt.Z_t, lp9.Z_t);
    }

    [Fact]
    public void GetLasPointRecordFormat10Test()
    {
        var lpt = _pointFaker.Generate();
        var header = _headerMock.Object;

        int expectedX = LasPoint.GetIntegerPosition(lpt.X, header);
        int expectedY = LasPoint.GetIntegerPosition(lpt.Y, header);
        int expectedZ = LasPoint.GetIntegerPosition(lpt.Z, header);

        var lp10 = LasPointRecordFormat10.GetLasPointStruct(lpt, header);

        Assert.Equal(expectedX, lp10.X);
        Assert.Equal(expectedY, lp10.Y);
        Assert.Equal(expectedZ, lp10.Z);
        Assert.Equal(lpt.Intensity, lp10.Intensity);
        Assert.Equal(lpt.GlobalEncoding, lp10.GlobalEncoding);
        Assert.Equal(lpt.Classification, lp10.Classification);
        Assert.Equal(lpt.ScanAngle, lp10.ScanAngle);
        Assert.Equal(lpt.UserData, lp10.UserData);
        Assert.Equal(lpt.FlightLine, lp10.FlightLine);
        Assert.Equal(lpt.Timestamp, lp10.Timestamp);
        Assert.Equal(lpt.R, lp10.R);
        Assert.Equal(lpt.G, lp10.G);
        Assert.Equal(lpt.B, lp10.B);
        Assert.Equal(lpt.NIR, lp10.NIR);
        Assert.Equal(lpt.WavePacketDescriptorIndex, lp10.WavePacketDescriptorIndex);
        Assert.Equal(lpt.ByteOffsetToWaveformData, lp10.ByteOffsetToWaveformData);
        Assert.Equal(lpt.WaveformPacketSizeBytes, lp10.WaveformPacketSizeBytes);
        Assert.Equal(lpt.ReturnPointWaveformLocation, lp10.ReturnPointWaveformLocation);
        Assert.Equal(lpt.X_t, lp10.X_t);
        Assert.Equal(lpt.Y_t, lp10.Y_t);
        Assert.Equal(lpt.Z_t, lp10.Z_t);
    }
}