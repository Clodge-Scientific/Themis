using System;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

namespace Themis.Las.Tests;

public class LasHeaderTests
{
    readonly Faker Faker = new();

    const double MinValue = -1337.0;
    const double MaxValue = 1337.0;

    const byte MinFormat = 0;
    const byte MaxFormat = 10;

    [Fact]
    public void SetFileSourceIDTest()
    {
        ushort ExpectedValue = Faker.Random.UShort();

        LasHeader Header = new();
        Header.SetFileSourceID(ExpectedValue);

        Assert.Equal(ExpectedValue, Header.FileSourceID);
    }

    [Fact]
    public void SetGlobalEncodingTest()
    {
        ushort ExpectedValue = Faker.Random.UShort();

        LasHeader Header = new();
        Header.SetGlobalEncoding(ExpectedValue);

        Assert.Equal(ExpectedValue, Header.GlobalEncoding);
    }

    [Fact]
    public void SetVersionTest()
    {
        byte ExpectedMajor = 1;
        byte ExpectedMinor = 2;

        byte BadMajor = 2;
        byte BadMinor = 3;

        LasHeader Header = new();
        Header.SetVersion(ExpectedMajor, ExpectedMinor);

        Assert.Equal(ExpectedMajor, Header.VersionMajor);
        Assert.Equal(ExpectedMinor, Header.VersionMinor);

        Assert.Throws<NotImplementedException>(() => Header.SetVersion(BadMajor, ExpectedMinor));
        Assert.Throws<NotImplementedException>(() => Header.SetVersion(ExpectedMajor, BadMinor));
    }

    [Fact]
    public void CheckExtremaTest()
    {
        var Minima = new double[] { MinValue, MinValue, MinValue };
        var Maxima = new double[] { MaxValue, MaxValue, MaxValue };

        LasHeader Header = new();
        Header.CheckExtrema(Minima);
        Header.CheckExtrema(Maxima);

        Assert.Equal(Header.MinX, MinValue);
        Assert.Equal(Header.MinY, MinValue);
        Assert.Equal(Header.MinZ, MinValue);

        Assert.Equal(Header.MaxX, MaxValue);
        Assert.Equal(Header.MaxY, MaxValue);
        Assert.Equal(Header.MaxZ, MaxValue);
    }

    [Fact]
    public void SetPointCountTest()
    {
        ulong ExpectedPointCount = Faker.Random.ULong();
        uint ExpectedLegacyPointCount = (uint)ExpectedPointCount;

        LasHeader Header = new();
        Header.SetPointCount(ExpectedPointCount);

        Assert.Equal(ExpectedPointCount, Header.PointCount);
        Assert.Equal(ExpectedPointCount, Header.NumPointRecords);
        Assert.Equal(ExpectedLegacyPointCount, Header.LegacyNumPointRecords);
    }

    [Fact]
    public void OnlyLegacyPointCountSetTest()
    {
        uint ExpectedLegacyPointCount = Faker.Random.UInt();
        ulong ExpectedNumPointRecords = 0;

        LasHeader Header = new();
        Header.SetLegacyNumPointRecords(ExpectedLegacyPointCount);

        Assert.Equal(ExpectedLegacyPointCount, Header.PointCount);
        Assert.Equal(ExpectedLegacyPointCount, Header.LegacyNumPointRecords);
        Assert.Equal(ExpectedNumPointRecords, Header.NumPointRecords);
    }

    [Fact]
    public void SetSystemIdentifierTest()
    {
        int Length = 32;
        string ExpectedString = Faker.Random.String(Length);
        char[] ExpectedValue = ExpectedString.ToCharArray();

        var HeaderA = new LasHeader();
        HeaderA.SetSystemIdentifier(ExpectedString);

        var HeaderB = new LasHeader();
        HeaderB.SetSystemIdentifier(ExpectedValue);

        Assert.Equal(ExpectedValue, HeaderA.SystemIdentifier);
        Assert.Equal(ExpectedValue, HeaderB.SystemIdentifier);
    }

    [Fact]
    public void SetGeneratingSoftwareTest()
    {
        int Length = 32;
        string ExpectedString = Faker.Random.String(Length);
        char[] ExpectedValue = ExpectedString.ToCharArray();

        var HeaderA = new LasHeader();
        HeaderA.SetGeneratingSoftware(ExpectedString);

        var HeaderB = new LasHeader();
        HeaderB.SetGeneratingSoftware(ExpectedValue);

        Assert.Equal(ExpectedValue, HeaderA.GeneratingSoftware);
        Assert.Equal(ExpectedValue, HeaderB.GeneratingSoftware);
    }

    [Fact]
    public void SetPointDataFormatTest()
    {
        byte ExpectedFormat = Faker.Random.Byte(MinFormat, MaxFormat);
        ushort ExpectedLength = PointTypeMap.GetPointRecordLength(ExpectedFormat);

        LasHeader Header = new();
        Header.SetPointDataFormat(ExpectedFormat);

        Assert.Equal(ExpectedFormat, Header.PointDataFormat);
        Assert.Equal(ExpectedLength, Header.PointDataRecordLength);
    }

    [Fact]
    public void SetNumVLRs()
    {
        uint ExpectedNumber = Faker.Random.UInt();

        LasHeader Header = new();
        Header.SetNumVLRs(ExpectedNumber);

        Assert.Equal(ExpectedNumber, Header.NumberOfVLRs);
    }

    [Fact]
    public void SetOffsetToPointData()
    {
        uint ExpectedNumber = Faker.Random.UInt();

        LasHeader Header = new();
        Header.SetOffsetToPointData(ExpectedNumber);

        Assert.Equal(ExpectedNumber, Header.OffsetToPointData);
    }

    [Fact]
    public void SetScaleTest()
    {
        double ExpectedX = Faker.Random.Double();
        double ExpectedY = Faker.Random.Double();
        double ExpectedZ = Faker.Random.Double();

        LasHeader HeaderA = new();
        HeaderA.SetScale(ExpectedX, ExpectedY, ExpectedZ);

        Assert.Equal(ExpectedX, HeaderA.ScaleX);
        Assert.Equal(ExpectedY, HeaderA.ScaleY);
        Assert.Equal(ExpectedZ, HeaderA.ScaleZ);

        LasHeader HeaderB = new();
        HeaderB.SetScale(HeaderA);

        Assert.Equal(ExpectedX, HeaderB.ScaleX);
        Assert.Equal(ExpectedY, HeaderB.ScaleY);
        Assert.Equal(ExpectedZ, HeaderB.ScaleZ);
    }

    [Fact]
    public void SetOriginTest()
    {
        double ExpectedX = Faker.Random.Double();
        double ExpectedY = Faker.Random.Double();
        double ExpectedZ = Faker.Random.Double();

        LasHeader HeaderA = new();
        HeaderA.SetOrigin(ExpectedX, ExpectedY, ExpectedZ);

        Assert.Equal(ExpectedX, HeaderA.OriginX);
        Assert.Equal(ExpectedY, HeaderA.OriginY);
        Assert.Equal(ExpectedZ, HeaderA.OriginZ);

        LasHeader HeaderB = new();
        HeaderB.SetOrigin(HeaderA);

        Assert.Equal(ExpectedX, HeaderB.OriginX);
        Assert.Equal(ExpectedY, HeaderB.OriginY);
        Assert.Equal(ExpectedZ, HeaderB.OriginZ);
    }

    [Fact]
    public void SetCreationDateTest()
    {
        //< February 11th, 2022 - the 42nd day of the year
        DateTime ExpectedDate = new DateTime(2022, 02, 11);
        ushort ExpectedYear = (ushort)ExpectedDate.Year;
        ushort ExpectedDOY = (ushort)ExpectedDate.DayOfYear;

        LasHeader HeaderA = new();
        HeaderA.SetCreationDate(ExpectedDate);

        LasHeader HeaderB = new();
        HeaderB.SetCreationDate(ExpectedYear, ExpectedDOY);

        Assert.Equal(ExpectedYear, HeaderA.CreationYear);
        Assert.Equal(ExpectedDOY, HeaderA.CreationDOY);
        Assert.Equal(ExpectedYear, HeaderB.CreationYear);
        Assert.Equal(ExpectedDOY, HeaderB.CreationDOY);
    }

    [Fact]
    public void SetMinimaTest()
    {
        double ExpectedX = Faker.Random.Double();
        double ExpectedY = Faker.Random.Double();
        double ExpectedZ = Faker.Random.Double();

        LasHeader Header = new();
        Header.SetMinima(ExpectedX, ExpectedY, ExpectedZ);

        Assert.Equal(ExpectedX, Header.MinX);
        Assert.Equal(ExpectedY, Header.MinY);
        Assert.Equal(ExpectedZ, Header.MinZ);
    }

    [Fact]
    public void SetMaximaTest()
    {
        double ExpectedX = Faker.Random.Double();
        double ExpectedY = Faker.Random.Double();
        double ExpectedZ = Faker.Random.Double();

        LasHeader Header = new();
        Header.SetMaxima(ExpectedX, ExpectedY, ExpectedZ);

        Assert.Equal(ExpectedX, Header.MaxX);
        Assert.Equal(ExpectedY, Header.MaxY);
        Assert.Equal(ExpectedZ, Header.MaxZ);
    }
}
