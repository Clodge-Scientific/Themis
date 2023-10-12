using Themis.Las.Stream;

namespace Themis.Las;

public class LasReader : ILasReader
{
    public bool EOF => _Stream.EOF;
    public ulong PointCount => Header.PointCount;
    public ILasHeader Header => _Stream.Header;
    public IList<LasVariableLengthRecord> VLRs => _Stream.VLRs;

    private readonly IStreamHandler _Stream;

    private bool _Disposing;
    private bool _Disposed;

    public LasReader(IStreamHandler stream)
    {
        _Stream = stream;
    }

    public LasReader(string lasFilePath, uint pointsToBuffer = Constants.DefaultReaderBufferCount)
    {
        _Stream = new AsyncStreamHandler(lasFilePath, pointsToBuffer).Initialize();
    }

    public LasPoint GetNextPoint()
        => _Stream.GetNextPoint();

    public void GetNextPoint(ref LasPoint lpt)
        => _Stream.GetNextPoint(ref lpt);

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        _Disposing = disposing;
        if (!_Disposed)
        {
            if (_Disposing) _Stream.Dispose();

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