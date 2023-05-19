namespace Themis.Las;

public interface ILasVariableLengthRecord
{
    /// <summary>
    /// Two leading reserved bytes of the VariableLengthRecord (VLR)
    /// </summary>
    ushort Reserved { get; }
    /// <summary>
    /// ID field indicating the type of VariableLengthRecord (VLR)
    /// </summary>
    ushort RecordID { get; }
    /// <summary>
    /// Total length (in bytes) of the VariableLengthRecord (VLR)
    /// </summary>
    ushort TotalRecordLength { get; }
    /// <summary>
    /// Total length (in bytes) of data following the header of the VariableLengthRecord (VLR)
    /// </summary>
    ushort RecordLengthAfterHeader { get; }

    /// <summary>
    /// 16-byte UserID field of the VariableLengthRecord (VLR)
    /// </summary>
    char[] UserID { get; }
    /// <summary>
    /// 32-byte Description field of the VariableLengthRecord (VLR)
    /// </summary>
    char[] Description { get; }
    /// <summary>
    /// The actual binary data stored within the VariableLengthRecord (VLR)
    /// </summary>
    byte[] Data { get; }

    /// <summary>
    /// Set the underlying 16-byte UserID field with the input string
    /// </summary>
    /// <param name="userID">string field to be set</param>
    void SetUserID(string userID);
    /// <summary>
    /// Set the underlying 16-byte UserID field with the input char[]
    /// </summary>
    /// <param name="userID">char[] field to be set</param>
    void SetUserID(char[] userID);
    /// <summary>
    /// Set the underlying 32-byte Description field with the input string
    /// </summary>
    /// <param name="description">string to set Description to</param>
    void SetDescription(string description);
    /// <summary>
    /// Set the underlying 32-byte Description field with the input char[]
    /// </summary>
    /// <param name="description">char[] to set description to</param>
    void SetDescription(char[] description);

    /// <summary>
    /// Fluently set the underlying 16-byte UserID field with the input string
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    ILasVariableLengthRecord WithUserID(string userID);
    /// <summary>
    /// Fluently set the underlying 16-byte UserID field with the input char[]
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    ILasVariableLengthRecord WithUserID(char[] userID);
    /// <summary>
    /// Fluently set the underlying 32-byte Description field with the input string
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    ILasVariableLengthRecord WithDescription(string description);
    /// <summary>
    /// Fluently set the underlying 32-byte Description field with the input char[]
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    ILasVariableLengthRecord WithDescription(char[] description);
}