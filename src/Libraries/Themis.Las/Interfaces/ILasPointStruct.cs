namespace Themis.Las;

/// <summary>
/// Represents the fundamental fields required for each LAS PointRecordFormat as stored in the LAS files 
/// </summary>
public interface ILasPointStruct : ILasPointBase
{
    /// <summary>
    /// Integer representation of LAS point's X-coordinate
    /// </summary>
    int X { get; }
    /// <summary>
    /// Integer representation of LAS point's Y-coordinate
    /// </summary>
    int Y { get; }
    /// <summary>
    /// Integer representation of LAS point's Z-coordinate
    /// </summary>
    int Z { get; }
}