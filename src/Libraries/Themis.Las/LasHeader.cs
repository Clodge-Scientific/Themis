namespace Themis.Las;

public class LasHeader : ILasHeader
{
    /// <summary>
    /// Maximum allowable length (in bytes) of the 'SystemIdentifier' and 'GeneratingSoftware' fields
    /// </summary>
    public const int IdentifierLength = 32;

    /// <summary>
    /// The currently supported 'VersionMinor' flags - assuming a 'VersionMajor' of 1
    /// </summary>
    public static readonly HashSet<int> SupportedVersionMinors = new() { 1, 2, 3, 4 };

    #region ILasHeader Fields & Properties
    public uint FileSignature { get; private set; } = Constants.LasHeaderSignature;

    public ushort FileSourceID { get; private set; } = 0;
    public ushort GlobalEncoding { get; private set; } = LasHelper.GetGlobalEncoding();

    public uint Guid1 { get; set; } = 0;
    public ushort Guid2 { get; set; } = 0;
    public ushort Guid3 { get; set; } = 0;
    public char[] Guid4 { get; set; } = new char[8];

    public byte VersionMajor { get; private set; } = 1;
    public byte VersionMinor { get; private set; } = 2;

    public char[] SystemIdentifier { get; private set; } = new char[IdentifierLength];
    public char[] GeneratingSoftware { get; private set; } = new char[IdentifierLength];

    public ushort CreationYear { get; private set; } = (ushort)DateTime.Now.Year;
    public ushort CreationDOY { get; private set; } = (ushort)DateTime.Now.DayOfYear;
    public ushort HeaderSize { get; private set; } = Constants.LasHeader12Size;

    public uint OffsetToPointData { get; private set; } = Constants.LasHeader12Size;
    public uint NumberOfVLRs { get; private set; } = 0;

    public byte PointDataFormat { get; private set; } = 0;
    public ushort PointDataRecordLength { get; private set; } = PointTypeMap.GetPointRecordLength(0);

    public uint LegacyNumPointRecords { get; private set; } = 0;
    public uint[] LegacyNumPointRecordsByReturn { get; set; } = new uint[5];

    public double ScaleX { get; private set; } = Constants.DefaultScale;
    public double ScaleY { get; private set; } = Constants.DefaultScale;
    public double ScaleZ { get; private set; } = Constants.DefaultScale;

    public double OriginX { get; private set; } = Constants.DefaultOffset;
    public double OriginY { get; private set; } = Constants.DefaultOffset;
    public double OriginZ { get; private set; } = Constants.DefaultOffset;

    public double MaxX { get; private set; } = double.MinValue;
    public double MinX { get; private set; } = double.MaxValue;
    public double MaxY { get; private set; } = double.MinValue;
    public double MinY { get; private set; } = double.MaxValue;
    public double MaxZ { get; private set; } = double.MinValue;
    public double MinZ { get; private set; } = double.MaxValue;

    public ulong StartOfWaveformDataPacketRecord { get; private set; } = 0;
    public ulong StartOfFirstExtendedVLR { get; private set; } = 0;
    public uint NumExtendedVLRs { get; private set; } = 0;
    public ulong NumPointRecords { get; private set; } = 0;
    public ulong[] NumPointRecordsByReturn { get; private set; } = new ulong[15];

    public ulong PointCount => NumPointRecords < LegacyNumPointRecords ? LegacyNumPointRecords : NumPointRecords;
    #endregion

    #region ILasHeader Methods
    public void SetPointCount(ulong count)
    {
        LegacyNumPointRecords = (uint)count;
        NumPointRecords = count;
    }

    public void SetLegacyNumPointRecords(uint count)
    {
        LegacyNumPointRecords = count;
    }

    public void SetSystemIdentifier(string systemIdentifier)
    {
        if (systemIdentifier == null) throw new ArgumentNullException(nameof(systemIdentifier));

        char[] sid = systemIdentifier.ToCharArray();
        SetSystemIdentifier(sid);
    }

    public void SetSystemIdentifier(char[] systemIdentifier)
    {
        if (systemIdentifier.Length > IdentifierLength)
            SystemIdentifier = systemIdentifier.Take(IdentifierLength).ToArray();
        else if (systemIdentifier.Length < IdentifierLength)
            SystemIdentifier = systemIdentifier.PadRight(IdentifierLength, Constants.DefaultChar);
        else
            SystemIdentifier = systemIdentifier;
    }

    public void SetGeneratingSoftware(string generatingSoftware)
    {
        if (generatingSoftware == null) throw new ArgumentNullException(nameof(generatingSoftware));

        char[] gen = generatingSoftware.ToCharArray();
        SetGeneratingSoftware(gen);
    }

    public void SetGeneratingSoftware(char[] generatingSoftware)
    {
        if (generatingSoftware.Length > IdentifierLength)
            GeneratingSoftware = generatingSoftware.Take(IdentifierLength).ToArray();
        else if (generatingSoftware.Length < IdentifierLength)
            GeneratingSoftware = generatingSoftware.PadRight(IdentifierLength, Constants.DefaultChar);
        else
            GeneratingSoftware = generatingSoftware;
    }

    public void CheckExtrema(IEnumerable<double> pos)
    {
        if (pos.Count() != 3) throw new ArgumentException($"Input position was not 3-Dimensional!");

        MinX = Math.Min(MinX, pos.ElementAt(0));
        MinY = Math.Min(MinY, pos.ElementAt(1));
        MinZ = Math.Min(MinZ, pos.ElementAt(2));

        MaxX = Math.Max(MaxX, pos.ElementAt(0));
        MaxY = Math.Max(MaxY, pos.ElementAt(1));
        MaxZ = Math.Max(MaxZ, pos.ElementAt(2));
    }

    public void SetPointDataFormat(byte format)
    {
        if (format < 0 || format > 10) throw new ArgumentException($"PointRecordFormat must be between range of [0, 10].");

        PointDataFormat = format;
        PointDataRecordLength = PointTypeMap.GetPointRecordLength(PointDataFormat);
    }

    public void SetNumVLRs(uint numVlrs)
    {
        if (numVlrs < 0) throw new ArgumentException($"Cannot set Number of Variable Length Records to a negative value");

        NumberOfVLRs = numVlrs;
    }

    public void SetOffsetToPointData(uint offset)
    {
        OffsetToPointData = offset;
    }

    public void SetScale(ILasHeader header)
    {
        SetScale(header.ScaleX, header.ScaleY, header.ScaleZ);
    }

    public void SetScale(double x, double y, double z)
    {
        ScaleX = x;
        ScaleY = y;
        ScaleZ = z;
    }

    public void SetOrigin(ILasHeader header)
    {
        SetOrigin(header.OriginX, header.OriginY, header.OriginZ);
    }

    public void SetOrigin(double x, double y, double z)
    {
        OriginX = x;
        OriginY = y;
        OriginZ = z;
    }

    public void SetFileSourceID(ushort id)
    {
        FileSourceID = id;
    }

    public void SetGlobalEncoding(ushort encoding)
    {
        GlobalEncoding = encoding;
    }

    public void SetVersion(byte major, byte minor)
    {
        if (major != 1) throw new NotImplementedException($"Euclid only supports 'VersionMajor' of {1}");
        if (!SupportedVersionMinors.Contains(minor)) throw new NotImplementedException($"Euclid does not support a 'VersionMinor' of {minor} - only {string.Join(", ", SupportedVersionMinors)}");
    }

    public void SetCreationDate(DateTime dt)
    {
        CreationYear = (ushort)dt.Year;
        CreationDOY = (ushort)dt.DayOfYear;
    }

    public void SetCreationDate(ushort year, ushort doy)
    {
        CreationYear = year;
        CreationDOY = doy;
    }

    public void SetMinima(double minX, double minY, double minZ)
    {
        MinX = minX;
        MinY = minY;
        MinZ = minZ;
    }

    public void SetMaxima(double maxX, double maxY, double maxZ)
    {
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;
    }
    #endregion

    #region Static Helper Methods
    public static LasHeader ReadFromPath(string path)
    {
        using var stream = new StreamReader(path);
        using var reader = new BinaryReader(stream.BaseStream);

        return ReadFromStream(reader);
    }

    public static LasHeader ReadFromStream(BinaryReader reader)
    {
        var header = new LasHeader
        {
            FileSignature = reader.ReadUInt32()
        };
        if (header.FileSignature != Constants.LasHeaderSignature) throw new Exception($"Invalid 'FileSignature' encountered: {header.FileSignature} - should only ever be {Constants.LasHeaderSignature}");

        header.FileSourceID = reader.ReadUInt16();
        header.GlobalEncoding = reader.ReadUInt16();
        header.Guid1 = reader.ReadUInt32();
        header.Guid2 = reader.ReadUInt16();
        header.Guid3 = reader.ReadUInt16();
        header.Guid4 = reader.ReadChars(8);
        header.VersionMajor = reader.ReadByte();
        header.VersionMinor = reader.ReadByte();

        if (header.VersionMajor != 1) throw new Exception($"Unknown 'VersionMajor' encountered: {header.VersionMajor}");
        if (!SupportedVersionMinors.Contains(header.VersionMinor)) throw new NotImplementedException($"Euclid does not support a 'VersionMinor' of {header.VersionMinor} - only {string.Join(", ", SupportedVersionMinors)}");

        header.SystemIdentifier = reader.ReadChars(32);
        header.GeneratingSoftware = reader.ReadChars(32);
        header.CreationDOY = reader.ReadUInt16();
        header.CreationYear = reader.ReadUInt16();
        header.HeaderSize = reader.ReadUInt16();
        header.OffsetToPointData = reader.ReadUInt32();
        header.NumberOfVLRs = reader.ReadUInt32();
        header.PointDataFormat = reader.ReadByte();
        header.PointDataRecordLength = reader.ReadUInt16();
        header.LegacyNumPointRecords = reader.ReadUInt32();
        for (int i = 0; i < header.LegacyNumPointRecordsByReturn.Length; i++)
        {
            header.LegacyNumPointRecordsByReturn[i] = reader.ReadUInt32();
        }
        header.ScaleX = reader.ReadDouble();
        header.ScaleY = reader.ReadDouble();
        header.ScaleZ = reader.ReadDouble();
        header.OriginX = reader.ReadDouble();
        header.OriginY = reader.ReadDouble();
        header.OriginZ = reader.ReadDouble();
        header.MaxX = reader.ReadDouble();
        header.MinX = reader.ReadDouble();
        header.MaxY = reader.ReadDouble();
        header.MinY = reader.ReadDouble();
        header.MaxZ = reader.ReadDouble();
        header.MinZ = reader.ReadDouble();

        //< LAS 1.1 & 1.2 header ends here
        if (header.VersionMinor <= 2) return header;

        header.StartOfWaveformDataPacketRecord = reader.ReadUInt64();

        //< LAS 1.3 header ends here
        if (header.VersionMinor == 3) return header;

        header.StartOfFirstExtendedVLR = reader.ReadUInt64();
        header.NumExtendedVLRs = reader.ReadUInt32();
        header.NumPointRecords = reader.ReadUInt64();
        for (int i = 0; i < header.NumPointRecordsByReturn.Length; i++)
        {
            header.NumPointRecordsByReturn[i] = reader.ReadUInt64();
        }

        return header;
    }
    #endregion
}