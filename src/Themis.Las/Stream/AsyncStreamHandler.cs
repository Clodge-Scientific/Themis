using System.Runtime.InteropServices;

namespace Themis.Las.Stream;

public class AsyncStreamHandler : IStreamHandler, IDisposable
{
    #region Public Fields
    public ulong ReadTicks { get; private set; } = 0;

    public ulong PointsRead { get; private set; } = 0;
    public ulong PointsYielded { get; private set; } = 0;
    public ulong PointsReturned { get; private set; } = 0;

    public long BytesRemaining => _StreamReader.BaseStream.Length - _StreamReader.BaseStream.Position;

    public uint BufferCount { get; private set; }
    public uint BufferSize { get; private set; }

    public Type PointType { get; private set; }

    public bool EOF => PointsReturned >= Header.PointCount;

    public ILasHeader Header { get; private set; } = null;
    public IList<LasVariableLengthRecord> VLRs { get; private set; } = new List<LasVariableLengthRecord>();
    #endregion

    #region Internal Fields / Streams
    private readonly StreamReader _StreamReader;
    private readonly BinaryReader _BinaryReader;

    private Thread _ReaderThread;
    private readonly AutoResetEvent _NeedDataEvent = new(false);
    private readonly AutoResetEvent _DataReadyEvent = new(false);

    private IStreamBuffer _A;
    private IStreamBuffer _B;
    private IStreamBuffer _Active;

    private byte[] _Buffer;

    private bool _Disposing;
    private bool _Disposed;
    #endregion

    public AsyncStreamHandler(string filePath, uint pointBufferCount = Constants.DefaultReaderBufferCount)
    {
        _StreamReader = new StreamReader(filePath);
        _BinaryReader = new BinaryReader(_StreamReader.BaseStream);

        BufferCount = pointBufferCount;
    }

    #region Fluent Interface
    public IStreamHandler Initialize()
    {
        //< Ensure the reader stream is set to the start
        _BinaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

        //< Parse the incoming ILasHeader and get the buffer size
        Header = LasHeader.ReadFromStream(_BinaryReader);

        BufferSize = BufferCount * Header.PointDataRecordLength;
        PointType = PointTypeMap.GetPointRecordType(Header);
        _Buffer = new byte[BufferSize];

        //< Parse any available LasVariableLengthRecords
        ReadVariableLengthRecords();

        //< Ensure we've 'seeked' the underlying stream to the start of point data
        Seek(Header.OffsetToPointData);

        //< Init (and start filling) the AsyncStreamHandler
        InitializeAsyncStreamHandler();

        return this;
    }
    #endregion

    #region Public Methods
    public void Seek(long pos)
    {
        if (pos < 0) throw new ArgumentOutOfRangeException(nameof(pos));
        if (pos > _BinaryReader.BaseStream.Length) throw new ArgumentOutOfRangeException(nameof(pos));

        _BinaryReader.BaseStream.Seek(pos, SeekOrigin.Begin);
    }

    public T ReadOfType<T>()
    {
        //< Get the number of bytes (by Type) and read the initial buffer into memory
        int count = Marshal.SizeOf(typeof(T));
        byte[] buff = _BinaryReader.ReadBytes(count);

        //< Generate the handle, marshal it, then free it
        GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
        T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();

        if (result is null) throw new InvalidOperationException($"Failed to read instance of type {typeof(T)} from stream.");

        //< Get the frig outta here with our object, m'lord(s)
        return result;
    }

    public LasPoint GetNextPoint()
    {
        CheckBufferStatus();
        return _Active.GetNext(Header);
    }

    public void GetNextPoint(ref LasPoint lpt)
    {
        CheckBufferStatus();
        _Active.GetNext(Header, ref lpt);
    }
    #endregion

    #region Internal Methods

    void ReadVariableLengthRecords()
    {
        if (Header.NumberOfVLRs == 0) return;

        VLRs = Enumerable.Range(0, (int)Header.NumberOfVLRs)
                         .Select(_ => LasVariableLengthRecord.ReadFromStream(_BinaryReader))
                         .ToList();
    }

    void InitializeAsyncStreamHandler()
    {
        _ReaderThread = new Thread(new ThreadStart(ReadPointsAsync));
        _ReaderThread.Start();

        _A = new AsyncStreamBuffer(BufferCount, PointType);
        _B = new AsyncStreamBuffer(BufferCount, PointType);
        SetActive(_A);
    }

    void CheckBufferStatus()
    {
        if (_Active.Available == 0)
        {
            _NeedDataEvent.Set();
            _DataReadyEvent.WaitOne();
        }
        PointsReturned++;
    }

    void SetActive(IStreamBuffer buff)
    {
        _Active = buff;
        PointsYielded += (ulong)_Active.Loaded;
    }

    void ReadPointsAsync()
    {
        while (PointsYielded < Header.PointCount && !_Disposing)
        {
            long startTicks = DateTime.Now.Ticks;
            _NeedDataEvent.WaitOne();

            //< Check if A or B have points available - set to active if they do
            if (_A.Available > 0)
            {
                SetActive(_A);
            }
            else if (_B.Available > 0)
            {
                SetActive(_B);
            }
            //< Neither AsyncStreamBuffer has points ready - use A, but also fill B
            else
            {
                MarshalPoints(_A);
                SetActive(_A);
                MarshalPoints(_B);
            }
            _DataReadyEvent.Set();

            //< There must be a cleaner way of doing this - can we wrap these up in an IEnumerable or something?
            if (_A.Available == 0 && _Active != _A) MarshalPoints(_A);
            if (_B.Available == 0 && _Active != _B) MarshalPoints(_B);

            ReadTicks += (ulong)(DateTime.Now.Ticks - startTicks);
        }
    }

    void MarshalPoints(IStreamBuffer buff)
    {
        if (BytesRemaining <= 0) return;

        //< Get number of point records remaining to be read, convert to number of bytes
        long remainingRecs = (long)(Header.PointCount - PointsRead);
        long toRead = Math.Min(Math.Min(BytesRemaining, BufferSize), remainingRecs * (long)Header.PointDataRecordLength);

        //< Read the bytes into the internal buffer
        _BinaryReader.Read(_Buffer, 0, (int)toRead);
        ulong numRead = (ulong)(toRead / Header.PointDataRecordLength);
        //< Get the memory address for the data array in the input IStreamBuffer
        var ph = GCHandle.Alloc(buff.Data, GCHandleType.Pinned);
        //< Copy directly to that address from internal buffer
        Marshal.Copy(_Buffer, 0, ph.AddrOfPinnedObject(), (int)toRead);
        ph.Free();

        //< Update tracking of both AsyncStreamHandler and IStreamBuffer
        buff.SetLoaded((int)numRead);
        buff.SetConsumed(0);
        PointsRead += numRead;
    }
    #endregion

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        _Disposing = disposing;
        if (!_Disposed)
        {
            if (_Disposing)
            {
                _ReaderThread.Join();
                if (_StreamReader != null)
                {
                    _StreamReader.Dispose();
                }
            }

            _Disposed = true;
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