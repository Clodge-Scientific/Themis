namespace Themis.Las.Stream;

public interface IStreamHandler : IDisposable
{
    /// <summary>
    /// Total number of ticks that have passed while actively reading from the underlying stream
    /// </summary>
    ulong ReadTicks { get; }

    /// <summary>
    /// Total number of point records that have been read from memory by the IStreamHandler
    /// </summary>
    ulong PointsRead { get; }
    /// <summary>
    /// Total number of points 'yielded' by underlying AsyncStreamBuffer(s)
    /// </summary>
    ulong PointsYielded { get; }
    /// <summary>
    /// The total number of points requested by the user of the IStreamHandler
    /// </summary>
    ulong PointsReturned { get; }

    /// <summary>
    /// The total number of remaining bytes in the stream (based on current stream position)
    /// </summary>
    long BytesRemaining { get; }

    /// <summary>
    /// The total number of points to be kept in each in-memory PointBuffer
    /// </summary>
    uint BufferCount { get; }
    /// <summary>
    /// The total number of bytes to be kept in each in-memory PointBuffer
    /// </summary>
    uint BufferSize { get; }

    /// <summary>
    /// Boolean flag indicating if we've read all the available point data
    /// </summary>
    bool EOF { get; }

    /// <summary>
    /// The type of LasPoint to be read by the reader
    /// </summary>
    Type PointType { get; }

    /// <summary>
    /// The ILasHeader from the ASPRS LAS file being parsed
    /// </summary>
    ILasHeader Header { get; }
    /// <summary>
    /// Any VariableLengthRecords that were stored between the ILasHeader and PointRecordData
    /// </summary>
    IList<LasVariableLengthRecord> VLRs { get; }

    /// <summary>
    /// Read, marshal and return a <typeparamref name="T"/> object from the underlying stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T ReadOfType<T>();
    /// <summary>
    /// Seek the underlying stream to the input position
    /// </summary>
    /// <param name="pos">Target position within the stream to seek to</param>
    void Seek(long pos);

    /// <summary>
    /// Read the next point record from memory and return it as a LasPoint
    /// </summary>
    /// <returns>LasPoint of next point record</returns>
    LasPoint GetNextPoint();
    /// <summary>
    /// Read the next point record from memory and update the input LasPoint
    /// </summary>
    /// <param name="lpt">LasPoint to be updated with the next point record</param>
    void GetNextPoint(ref LasPoint lpt);

    /// <summary>
    /// Get the IStreamHandler ready for first-point-read by pre-reading the ILasHeader and any available ILasVariableLengthRecords
    /// </summary>
    /// <returns>Initialized IStreamHandler instance</returns>
    IStreamHandler Initialize();
}