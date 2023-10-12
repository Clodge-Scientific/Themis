using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Themis.Las.Structs;

namespace Themis.Las;

public class LasWriter : ILasWriter
{
    private bool disposedValue;

    private readonly StreamWriter _streamWriter;
    private readonly BinaryWriter _binaryWriter;

    public ILasHeader Header { get; private set; }
    public IEnumerable<LasVariableLengthRecord> VLRs { get; private set; }

    public string OutputFile { get; private set; } = string.Empty;

    public ulong PointsWritten { get; private set; } = 0;
    public long Position => _binaryWriter.BaseStream.Position;

    public LasWriter(string lasFile, ILasHeader header, IEnumerable<LasVariableLengthRecord>? vlrs = null)
    {
        Header = header;
        VLRs = vlrs ?? new List<LasVariableLengthRecord>();
        OutputFile = lasFile;

        //< Check that the 'Offset to Point Data' makes sense
        var dataOffset = CalculateOffsetToPointData(Header, VLRs);
        if (dataOffset != Header.OffsetToPointData)
        {
            Header.SetOffsetToPointData(dataOffset);
        }

        _streamWriter = new StreamWriter(File.Open(lasFile, FileMode.Create));
        _binaryWriter = new BinaryWriter(_streamWriter.BaseStream);
    }

    public ILasWriter Initialize()
    {
        WriteHeader();
        if (VLRs != null) WriteVLRs(VLRs);

        return this;
    }

    /// <summary>
    /// Calculate the 'actual' offset to point data given <see cref="ILasHeader"/> and optional set of <see cref="LasVariableLengthRecord"/>
    /// </summary>
    /// <param name="header">Output <see cref="ILasHeader"/> to be written</param>
    /// <param name="vlrs">[Optional] Set of <see cref="LasVariableLengthRecord"/></param>
    /// <returns></returns>
    static ushort CalculateOffsetToPointData(ILasHeader header, IEnumerable<LasVariableLengthRecord>? vlrs = null)
    {
        ushort size = header.HeaderSize;

        if (vlrs != null) size += (ushort)vlrs.Sum(x => x.TotalRecordLength);

        return size;
    }

    /// <summary>
    /// Seek the underlying binary output stream to the input position
    /// </summary>
    /// <param name="position">Position to set binary output stream to</param>
    long Seek(int position)
    {
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
        if (position > _binaryWriter.BaseStream.Length) throw new ArgumentOutOfRangeException(nameof(position));

        return _binaryWriter.Seek(position, SeekOrigin.Begin);
    }

    /// <summary>
    /// Output the <see cref="ILasWriter"/>'s current <see cref="ILasHeader"/> to the binary output stream
    /// <para>NOTE: Will re-set the <see cref="ILasWriter"/>'s position to just before the Variable Length Record (VLR) block</para>
    /// </summary>
    void WriteHeader()
    {
        Seek(0);
        Header.WriteToFile(_binaryWriter);
    }

    /// <summary>
    /// Output the given set of <see cref="LasVariableLengthRecord"/> to the binary output stream
    /// <para>NOTE: Will re-set the <see cref="ILasWriter"/>'s position to just before the point data block</para>
    /// </summary>
    /// <param name="vlrs"></param>
    void WriteVLRs(IEnumerable<LasVariableLengthRecord> vlrs)
    {
        if (Position < Header.HeaderSize) Seek(Header.HeaderSize);

        foreach (var vlr in vlrs) vlr.WriteToFile(_binaryWriter);
    }

    void CheckAgainstHeader(LasPoint point)
    {
        PointsWritten++;
        Header.CheckExtrema(point.Position);
    }

    public void WritePoint(LasPoint point)
    {
        CheckAgainstHeader(point);

        var lpt = LasPointConverter.GetLasPointStruct(point, Header);
        _binaryWriter.Write(GetBytes(lpt));
    }
    public void WritePoints(IEnumerable<LasPoint> points)
    {
        foreach (var point in points) CheckAgainstHeader(point);

        var structs = points.Select(p => LasPointConverter.GetLasPointStruct(p, Header));
        _binaryWriter.Write(GetBytes(structs));
    }

    static byte[] GetBytes<T>(T input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        int length = Marshal.SizeOf(input);
        IntPtr ptr = Marshal.AllocHGlobal(length);
        byte[] myBuffer = new byte[length];

        Marshal.StructureToPtr(input, ptr, true);
        Marshal.Copy(ptr, myBuffer, 0, length);
        Marshal.FreeHGlobal(ptr);

        return myBuffer;
    }

    static byte[] GetBytes<T>(IEnumerable<T> inputs)
    {
        int structSize = Marshal.SizeOf(inputs.First());
        int len = inputs.Count() * structSize;
        byte[] arr = new byte[len];
        IntPtr ptr = Marshal.AllocHGlobal(len);
        for (int i = 0; i < inputs.Count(); i++)
        {
            Marshal.StructureToPtr(
                inputs.ElementAt(i) ?? throw new NullReferenceException(),
                ptr + i * structSize,
                true);
        }
        Marshal.Copy(ptr, arr, 0, len);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                //< Verify the point count matches what we actually wrote
                if (Header.PointCount != PointsWritten)
                {
                    Header.SetPointCount(PointsWritten);
                }
                //< Re-write the header to ensure count & bounding boxes are legit
                WriteHeader();
                //< Dispose underlying stream
                _streamWriter.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}