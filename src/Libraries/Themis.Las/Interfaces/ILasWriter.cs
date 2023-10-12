namespace Themis.Las;

public interface ILasWriter : IDisposable
{
    /// <summary>
    /// The ILasHeader object to be written to the output LAS file
    /// </summary>
    public ILasHeader Header { get; }
    /// <summary>
    /// A collection of all <see cref="LasVariableLengthRecord"/> entities to be written to the output stream
    /// </summary>
    public IEnumerable<LasVariableLengthRecord> VLRs { get; }

    /// <summary>
    /// The total count of <see cref="LasPoint"/> records written to the underlying output stream
    /// </summary>
    public ulong PointsWritten { get; }
    /// <summary>
    /// The current position of the underlying binary output stream
    /// </summary>
    public long Position { get; }

    /// <summary>
    /// The fully-composed file path to the output file on disk
    /// </summary>
    public string OutputFile { get; }

    /// <summary>
    /// Output the current <see cref="ILasHeader"/> and any <see cref="LasVariableLengthRecord"/> entities and prepare stream for writing <see cref="LasPoint"/> data
    /// </summary>
    /// <returns>Initialized <see cref="ILasWriter"/></returns>
    ILasWriter Initialize();

    /// <summary>
    /// Write the given <see cref="LasPoint"/> object to the binary output stream at the current position
    /// </summary>
    /// <param name="point"><see cref="LasPoint"/> to be output</param>
    void WritePoint(LasPoint point);
    /// <summary>
    /// Write the given set of <see cref="LasPoint"/> objects to the binary output stream at the current position
    /// </summary>
    /// <param name="points"><see cref="LasPoint"/> objects to be output</param>
    void WritePoints(IEnumerable<LasPoint> points);
}