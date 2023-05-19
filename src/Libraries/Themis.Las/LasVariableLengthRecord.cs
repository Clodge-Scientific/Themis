using System.Text;

namespace Themis.Las;

public class LasVariableLengthRecord : ILasVariableLengthRecord
{
    public static readonly ushort HeaderLength = 54;

    public const char DefaultChar = '\0';

    public const int MaxUserIDLength = 16;
    public const int MaxDescriptionLength = 32;

    public const int ProjectionReserved = 0;
    public const int ProjectionRecordID = 2112;
    public const string ProjectionUserID = "LASF_Projection";
    public const string ProjectionDescription = "Projection";

    public ushort Reserved { get; set; } = 0;
    public ushort RecordID { get; set; } = ProjectionRecordID;
    public ushort TotalRecordLength => (ushort)(Data.Length + HeaderLength);
    public ushort RecordLengthAfterHeader { get; set; } = 0;

    public char[] UserID { get; private set; } = new char[MaxUserIDLength];
    public char[] Description { get; private set; } = new char[MaxDescriptionLength];
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public void SetUserID(string userID)
    {
        if (userID.Length > MaxUserIDLength) throw new($"UserID must be no more than 16 characters in length");

        this.UserID = userID.PadRight(MaxUserIDLength, DefaultChar).ToCharArray();
    }

    public void SetUserID(char[] userID)
    {
        if (userID.Length > MaxUserIDLength) throw new($"UserID must be no more than 16 characters in length");
        if (userID.Length < MaxUserIDLength) userID = userID.PadRight(MaxUserIDLength, DefaultChar);

        this.UserID = userID;
    }

    public void SetDescription(string description)
    {
        if (description.Length > MaxDescriptionLength) throw new($"Description must be no more than 32 characters in length");

        this.Description = description.PadRight(MaxDescriptionLength, DefaultChar).ToCharArray();
    }

    public void SetDescription(char[] description)
    {
        if (description.Length > MaxDescriptionLength) throw new($"Description must be no more than 32 characters in length");
        if (description.Length < MaxDescriptionLength) description = description.PadRight(MaxDescriptionLength, DefaultChar);

        this.Description = description;
    }

    public ILasVariableLengthRecord WithUserID(string userID)
    {
        SetUserID(userID);
        return this;
    }

    public ILasVariableLengthRecord WithUserID(char[] userID)
    {
        SetUserID(userID);
        return this;
    }

    public ILasVariableLengthRecord WithDescription(string description)
    {
        SetDescription(description);
        return this;
    }

    public ILasVariableLengthRecord WithDescription(char[] description)
    {
        SetDescription(description);
        return this;
    }

    public static LasVariableLengthRecord ReadFromStream(BinaryReader reader)
    {
        //< Parse inital fields to determine length of record after header
        var vlr = new LasVariableLengthRecord()
        {
            Reserved = reader.ReadUInt16(),
            UserID = reader.ReadChars(16),
            RecordID = reader.ReadUInt16(),
            RecordLengthAfterHeader = reader.ReadUInt16(),
            Description = reader.ReadChars(32)
        };
        //< Grab the remaining binary data and return
        vlr.Data = reader.ReadBytes(vlr.RecordLengthAfterHeader);
        return vlr;
    }

    public static LasVariableLengthRecord GetProjectionVLR(string projString)
    {
        var vlr = new LasVariableLengthRecord
        {
            Reserved = ProjectionReserved,
            RecordID = ProjectionRecordID
        };

        vlr.SetUserID(ProjectionUserID);
        vlr.SetDescription(ProjectionDescription);

        vlr.Data = GetProjectionData(projString);
        vlr.RecordLengthAfterHeader = (ushort)vlr.Data.Length;

        return vlr;
    }

    public static byte[] GetProjectionData(string projString)
    {
        //< Projection needs to be null-terminated and is assumed UTF-8
        var projData = projString.PadRight(projString.Length + 1, DefaultChar);
        return Encoding.UTF8.GetBytes(projData);
    }
}