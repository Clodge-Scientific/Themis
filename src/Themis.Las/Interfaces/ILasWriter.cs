namespace Themis.Las;

public interface ILasWriter : IDisposable
{
    /// <summary>
    /// The ILasHeader object to be written to the output LAS file
    /// </summary>
    public ILasHeader Header { get; }

    /// <summary>
    /// The current position of the underlying binary output stream
    /// </summary>
    public long Position { get; }

    /// <summary>
    /// Seek the underlying binary output stream to the input position
    /// </summary>
    /// <param name="position">Position to set binary output stream to</param>
    long Seek(int position);

    /// <summary>
    /// Output the ILasWriter's current ILasHeader to the binary output stream
    /// <para>NOTE: Will re-set the ILasWriter's position to just before the Variable Length Record (VLR) block</para>
    /// </summary>
    void WriteHeader();
    /// <summary>
    /// Update the ILasWriter's current ILasHeader to the input ILasHeader, then output it
    /// <para>NOTE: Will re-set the ILasWriter's position to just before the Variable Length Record (VLR) block</para>
    /// </summary>
    /// <param name="header">ILasHeader object to be output and used</param>
    void WriteHeader(ILasHeader header);

    /// <summary>
    /// Output the given ILasVariableLengthRecord to the binary output stream
    /// <para>NOTE: Will re-set the ILasWriter's position to just before the point data block</para>
    /// </summary>
    /// <param name="vlr">ILasVariableLengthRecord to be output</param>
    void WriteVLR(ILasVariableLengthRecord vlr);
    /// <summary>
    /// Output the given set of ILasVariableLengthRecords to the binary output stream
    /// <para>NOTE: Will re-set the ILasWriter's position to just before the point data block</para>
    /// </summary>
    /// <param name="vlrs"></param>
    void WriteVLRs(IEnumerable<ILasVariableLengthRecord> vlrs);

    /// <summary>
    /// Write the given ILasPoint object to the binary output stream at the current position
    /// </summary>
    /// <param name="point">ILasPoint to be output</param>
    void WritePoint(ILasPoint point);
    /// <summary>
    /// Write the given set of ILasPoint objects to the binary output stream at the current position
    /// </summary>
    /// <param name="points">ILasPoint objects to be output</param>
    void WritePoints(IEnumerable<ILasPoint> points);

    /// <summary>
    /// Write the given byte[] to the binary output stream at the current position
    /// </summary>
    /// <param name="bytes"></param>
    void WriteBytes(byte[] bytes);
}