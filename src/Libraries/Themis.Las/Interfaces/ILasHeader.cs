namespace Themis.Las;

public interface ILasHeader
{
    #region ASPRS Specification Fields
    /// <summary>
    /// 4-byte 'LASF' character array that starts each LAS file
    /// </summary>
    uint FileSignature { get; }
    /// <summary>
    /// ID assigned to the LAS file based on generating flightline in range [0, 65535]
    /// </summary>
    ushort FileSourceID { get; }
    /// <summary>
    /// Bit field used to indicate timestamp convention (SOW vs GPS Standard) and use of projection WKT in VLRs
    /// </summary>
    ushort GlobalEncoding { get; }

    /// <summary>
    /// [Optional] first (uint) component of the four Project Identifier GUID fields
    /// </summary>
    uint Guid1 { get; }
    /// <summary>
    /// [Optional] second (ushort) component of the four Project Identifier GUID fields
    /// </summary>
    ushort Guid2 { get; }
    /// <summary>
    /// [Optional] third (ushort) component of the four Project Identifier GUID fields
    /// </summary>
    ushort Guid3 { get; }
    /// <summary>
    /// [Optional] fourth (char[]) component of the four Project Identifier GUID fields
    /// </summary>
    char[] Guid4 { get; }

    /// <summary>
    /// The 'Major' version of the LAS file - should always be set to '1' (as in LAS 1.2)
    /// </summary>
    byte VersionMajor { get; }
    /// <summary>
    /// The 'Minor' version of the LAS file - should be between [0, 4] but we only support 2 & 4
    /// </summary>
    byte VersionMinor { get; }

    /// <summary>
    /// 32-character string that identifies how a LAS file was generated (whether by sensor, or by merge, etc)
    /// </summary>
    char[] SystemIdentifier { get; }
    /// <summary>
    /// 32-character string that identifies what software was used to generate the LAS file
    /// </summary>
    char[] GeneratingSoftware { get; }

    /// <summary>
    /// The day-of-year (in range [1, 366]) the LAS file was created
    /// </summary>
    ushort CreationDOY { get; }
    /// <summary>
    /// The four-digit year the LAS file was generated
    /// </summary>
    ushort CreationYear { get; }

    /// <summary>
    /// The size, in bytes, of the Public Header Block
    /// </summary>
    ushort HeaderSize { get; }
    /// <summary>
    /// The offset, in bytes, from the start of the file to the first Point Data Record
    /// </summary>
    uint OffsetToPointData { get; }
    /// <summary>
    /// The total integer number of Variable Length Records between the Public Header Block and Point Data Records
    /// </summary>
    uint NumberOfVLRs { get; }
    /// <summary>
    /// Byte flag in range of [0, 10] that indicates the Point Data Format (PDF) or contained Point Data Records
    /// </summary>
    byte PointDataFormat { get; }
    /// <summary>
    /// The length, in bytes, of Point Data Records stored within the LAS file
    /// </summary>
    ushort PointDataRecordLength { get; }
    /// <summary>
    /// The legacy (LAS1.2) count of contained point records stored as a 32-bit unsigned integer
    /// </summary>
    uint LegacyNumPointRecords { get; }
    /// <summary>
    /// The legacy (LAS1.2) histogram of total point records by return
    /// <para>Allows for up to five (5) returns per pulse</para>
    /// </summary>
    uint[] LegacyNumPointRecordsByReturn { get; }

    /// <summary>
    /// Scale factor to be applied to a position's X-coordinate when converting from floating point to integer for storage
    /// <para>X_coordinate = (X_record * X_scale) + X_offset</para>
    /// </summary>
    double ScaleX { get; }
    /// <summary>
    /// Scale factor to be applied to a position's Y-coordinate when converting from floating point to integer for storage
    /// <para>Y_coordinate = (Y_record * Y_scale) + Y_offset</para>
    /// </summary>
    double ScaleY { get; }
    /// <summary>
    /// Scale factor to be applied to a position's Z-coordinate when converting from floating point to integer for storage
    /// <para>Z_coordinate = (Z_record * Z_scale) + Z_offset</para>
    /// </summary>
    double ScaleZ { get; }

    /// <summary>
    /// The spatial offset to be applied to a position's X-coordinate from converting from floating point to integer for storage
    /// <para>X_coordinate = (X_record * X_scale) + X_offset</para>
    /// </summary>
    double OriginX { get; }
    /// <summary>
    /// The spatial offset to be applied to a position's Y-coordinate from converting from floating point to integer for storage
    /// <para>Y_coordinate = (Y_record * Y_scale) + Y_offset</para>
    /// </summary>
    double OriginY { get; }
    /// <summary>
    /// The spatial offset to be applied to a position's Z-coordinate from converting from floating point to integer for storage
    /// <para>Z_coordinate = (Z_record * Z_scale) + Z_offset</para>
    /// </summary>
    double OriginZ { get; }

    /// <summary>
    /// The maximum X-coordinate of point data contained within the LAS file
    /// </summary>
    double MaxX { get; }
    /// <summary>
    /// The minimum X-coordinate of point data contained within the LAS file
    /// </summary>
    double MinX { get; }
    /// <summary>
    /// The maximum Y-coordinate of point data contained within the LAS file
    /// </summary>
    double MaxY { get; }
    /// <summary>
    /// The minimum Y-coordinate of point data contained within the LAS file
    /// </summary>
    double MinY { get; }
    /// <summary>
    /// The maximum Z-coordinate of point data contained within the LAS file
    /// </summary>
    double MaxZ { get; }
    /// <summary>
    /// The minimum Z-coordinate of point data contained within the LAS file
    /// </summary>
    double MinZ { get; }

    /// <summary>
    /// The offset (in bytes) to the first byte of the Waveform Data Package Record
    /// </summary>
    ulong StartOfWaveformDataPacketRecord { get; }
    /// <summary>
    /// The offset (in bytes) to the first byte of the first Extended Variable Length Record
    /// </summary>
    ulong StartOfFirstExtendedVLR { get; }
    /// <summary>
    /// The current number of Extended Variable Length Records that are stored within the file after the Point Data Records
    /// </summary>
    uint NumExtendedVLRs { get; }
    /// <summary>
    /// The total number of Point Data Records stored within the LAS file (as 64-bit unsigned long integer)
    /// </summary>
    ulong NumPointRecords { get; }
    /// <summary>
    /// The histogram of total point records by return number
    /// <para>Allows for up to fifteen (15) returns per pulse</para>
    /// </summary>
    ulong[] NumPointRecordsByReturn { get; }
    #endregion

    #region ILasHeader Fields
    /// <summary>
    /// The absolute point count currently tracked by this LasHeader
    /// <para>NOTE: This is the 'true' count so you don't have to choose between legacy and modern count</para>
    /// </summary>
    ulong PointCount { get; }
    #endregion

    #region ILasHeader Methods
    /// <summary>
    /// Update the point count fields within the Public Header Block
    /// <para>NOTE: This will update both 'LegacyNumPointRecords' and 'NumPointRecords'</para>
    /// </summary>
    /// <param name="count"></param>
    void SetPointCount(ulong count);
    /// <summary>
    /// Update the 'LegacyNumPointRecords' field to the input value
    /// <para>NOTE: This only updates the 'LegacyNumPointRecords' field</para>
    /// </summary>
    /// <param name="count">Count of points to be set</param>
    void SetLegacyNumPointRecords(uint count);
    /// <summary>
    /// Set the 'FileSourceID' field to the input ushort value
    /// </summary>
    /// <param name="id">FileSourceID value to be used</param>
    void SetFileSourceID(ushort id);
    /// <summary>
    /// Set the 'GlobalEncoding' bit flag to the input ushort value
    /// <para>NOTE: This flag indicates timestamp convention and coordinate CRS, don't fuck it up</para>
    /// </summary>
    /// <param name="encoding">GlobalEncoding value to be used</param>
    void SetGlobalEncoding(ushort encoding);
    /// <summary>
    /// Update the 'VersionMajor' and 'VersionMinor' flags to the input values
    /// <para>NOTE: This will update the 'HeaderSize' field to match the specified version</para>
    /// </summary>
    /// <param name="major">The 'VersionMajor' value to be used - currently must be set to '1'</param>
    /// <param name="minor">The 'VersionMinor' value to be used - currently only 2 & 4 are supported</param>
    void SetVersion(byte major, byte minor);
    /// <summary>
    /// Set the 'SystemIdentifier' field to the input string
    /// <para>NOTE: Will enforce a 32-character limit - larger strings will be truncated</para>
    /// </summary>
    /// <param name="systemIdentifier">string to be used for the 'SystemIdentifier' field</param>
    void SetSystemIdentifier(string systemIdentifier);
    /// <summary>
    /// Set the 'SystemIdentifier' field to the input char[]
    /// <para>NOTE: Will enforce a 32-character limit - larger arrays will be truncated</para>
    /// </summary>
    /// <param name="systemIdentifier">char[] to be used for the 'SystemIdentifier' field</param>
    void SetSystemIdentifier(char[] systemIdentifier);
    /// <summary>
    /// Set the 'GeneratingSoftware' field to the input string
    /// <para>NOTE: Will enforce a 32-character limit - larger strings will be truncated</para>
    /// </summary>
    /// <param name="generatingSoftware">string to be used for the 'GeneratingSoftware' field</param>
    void SetGeneratingSoftware(string generatingSoftware);
    /// <summary>
    /// Set the 'GeneratingSoftware' field to the input char[]
    /// <para>NOTE: Will enforce a 32-character limit - larger arrays will be truncated</para>
    /// </summary>
    /// <param name="generatingSoftware">char[] to be used for the 'GeneratingSoftware' field</param>
    void SetGeneratingSoftware(char[] generatingSoftware);
    /// <summary>
    /// Set the 'CreationYear' and 'CreationDOY' fields to match the input DateTime object
    /// </summary>
    /// <param name="dt">DateTime object to be used</param>
    void SetCreationDate(DateTime dt);
    /// <summary>
    /// Set the 'CreationYear' and 'CreationDOY' fields to the input year and day-of-year values
    /// </summary>
    /// <param name="year">Four-digit year</param>
    /// <param name="doy">Integer day-of-year in range [1, 366]</param>
    void SetCreationDate(ushort year, ushort doy);
    /// <summary>
    /// Update the number of Variable Length Records stored after the Public Header Block
    /// <para>NOTE: Does not update 'Offset to Point Data'</para>
    /// </summary>
    /// <param name="numVlrs">Number of Variable Length Records</param>
    void SetNumVLRs(uint numVlrs);
    /// <summary>
    /// Update the Point Data Format to the given format specification
    /// <para>NOTE: Will also update the 'Point Data Record Length' field</para>
    /// </summary>
    /// <param name="format"></param>
    void SetPointDataFormat(byte format);
    /// <summary>
    /// Update the 'Offset to Point Data' field to the given unsigned integer (in bytes)
    /// </summary>
    /// <param name="offset"></param>
    void SetOffsetToPointData(uint offset);

    /// <summary>
    /// Check & update the ILasHeader's minima & maxima against the input position's dimensional values
    /// </summary>
    /// <param name="pos"></param>
    void CheckExtrema(IEnumerable<double> pos);
    /// <summary>
    /// Set the minima values to the input dimensional values
    /// </summary>
    /// <param name="minX">Minimum X-coordinate value</param>
    /// <param name="minY">Minimum Y-coordinate value</param>
    /// <param name="minZ">Minimum Z-coordinate value</param>
    void SetMinima(double minX, double minY, double minZ);
    /// <summary>
    /// Set the maxima values to the input dimensional values
    /// </summary>
    /// <param name="maxX">Maximum X-coordinate value</param>
    /// <param name="maxY">Maximum Y-coordinate value</param>
    /// <param name="maxZ">Maximum Z-coordinate value</param>
    void SetMaxima(double maxX, double maxY, double maxZ);

    /// <summary>
    /// Update the ILasHeader's 'Scale' fields to match the input ILasHeader's
    /// </summary>
    /// <param name="header">ILasHeader to pull 'Scale' values from</param>
    void SetScale(ILasHeader header);
    /// <summary>
    /// Update the ILasHeader's 'Scale' fields to the input (X,Y,Z) values
    /// </summary>
    /// <param name="x">ScaleX value to be used</param>
    /// <param name="y">ScaleY value to be used</param>
    /// <param name="z">ScaleZ value to be used</param>
    void SetScale(double x, double y, double z);
    /// <summary>
    /// Update the ILasHeader's 'Origin' fields to match the input ILasHeader's
    /// </summary>
    /// <param name="header">ILasHeader to pull 'Origin' values from</param>
    void SetOrigin(ILasHeader header);
    /// <summary>
    /// Update the ILasHeader's 'Origin' fields to the input (X,Y,Z) values
    /// </summary>
    /// <param name="x">OriginX value to be used</param>
    /// <param name="y">OriginY value to be used</param>
    /// <param name="z">OriginZ value to be used</param>
    void SetOrigin(double x, double y, double z);
    #endregion
}