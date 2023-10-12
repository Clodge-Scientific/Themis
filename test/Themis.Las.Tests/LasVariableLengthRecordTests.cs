using Bogus;
using Xunit;
using Assert = Xunit.Assert;

namespace Themis.Las.Tests;

public class LasVariableLengthRecordTests
{
    readonly Faker Faker = new();

    [Fact]
    public void SetUserIDTest()
    {
        int expectedLength = LasVariableLengthRecord.MaxUserIDLength;
        var userID = Faker.Random.String(expectedLength);

        var vlrA = new LasVariableLengthRecord();
        var vlrB = new LasVariableLengthRecord();
        vlrA.SetUserID(userID);
        vlrB.SetUserID(userID.ToCharArray());

        Assert.Equal(userID.ToCharArray(), vlrA.UserID);
        Assert.Equal(userID.ToCharArray(), vlrB.UserID);
    }

    [Fact]
    public void WithUserIDTest()
    {
        int expectedLength = LasVariableLengthRecord.MaxUserIDLength;
        var userID = Faker.Random.String(expectedLength);

        var vlrA = new LasVariableLengthRecord().WithUserID(userID);
        var vlrB = new LasVariableLengthRecord().WithUserID(userID.ToCharArray());

        Assert.Equal(userID.ToCharArray(), vlrA.UserID);
        Assert.Equal(LasVariableLengthRecord.MaxUserIDLength, vlrA.UserID.Length);
        Assert.Equal(userID.ToCharArray(), vlrB.UserID);
        Assert.Equal(LasVariableLengthRecord.MaxUserIDLength, vlrB.UserID.Length);
    }

    [Fact]
    public void SetDescriptionTest()
    {
        int expectedLength = LasVariableLengthRecord.MaxDescriptionLength;
        var description = Faker.Random.String(expectedLength);

        var vlrA = new LasVariableLengthRecord();
        var vlrB = new LasVariableLengthRecord();
        vlrA.SetDescription(description);
        vlrB.SetDescription(description.ToCharArray());

        Assert.Equal(description.ToCharArray(), vlrA.Description);
        Assert.Equal(LasVariableLengthRecord.MaxDescriptionLength, vlrA.Description.Length);
        Assert.Equal(description.ToCharArray(), vlrB.Description);
        Assert.Equal(LasVariableLengthRecord.MaxDescriptionLength, vlrB.Description.Length);
    }

    [Fact]
    public void WithDescriptionTest()
    {
        int expectedLength = LasVariableLengthRecord.MaxDescriptionLength;
        var description = Faker.Random.String(expectedLength);

        var vlrA = new LasVariableLengthRecord().WithDescription(description);
        var vlrB = new LasVariableLengthRecord().WithDescription(description.ToCharArray());

        Assert.Equal(description.ToCharArray(), vlrA.Description);
        Assert.Equal(LasVariableLengthRecord.MaxDescriptionLength, vlrA.Description.Length);
        Assert.Equal(description.ToCharArray(), vlrB.Description);
        Assert.Equal(LasVariableLengthRecord.MaxDescriptionLength, vlrB.Description.Length);
    }

    [Fact]
    public void GetProjectionVlrTest()
    {
        var projString = Faker.Lorem.Word();

        //< Length is padded by 1 byte as the projection data blob must be null terminated
        int expectedLengthAfterHeader = projString.Length + 1;
        int expectedRecordLength = LasVariableLengthRecord.HeaderLength + expectedLengthAfterHeader;

        byte[] expectedData = LasVariableLengthRecord.GetProjectionData(projString);

        var vlr = LasVariableLengthRecord.GetProjectionVLR(projString);

        string actualUserID = new string(vlr.UserID).Trim(LasVariableLengthRecord.DefaultChar);
        string actualDescription = new string(vlr.Description).Trim(LasVariableLengthRecord.DefaultChar);

        Assert.Equal(LasVariableLengthRecord.ProjectionReserved, vlr.Reserved);
        Assert.Equal(LasVariableLengthRecord.ProjectionRecordID, vlr.RecordID);
        Assert.Equal(LasVariableLengthRecord.ProjectionUserID, actualUserID);
        Assert.Equal(LasVariableLengthRecord.ProjectionDescription, actualDescription);

        Assert.Equal(expectedLengthAfterHeader, vlr.RecordLengthAfterHeader);
        Assert.Equal(expectedRecordLength, vlr.TotalRecordLength);
        Assert.Equal(expectedData, vlr.Data);
    }
}