using System;
using System.IO;
using System.Collections.Generic;

namespace Themis.Las;

public class LasWriter : ILasWriter
{
    private bool disposedValue;

    private readonly StreamWriter _StreamWriter;
    private readonly BinaryWriter _BinaryWriter;

    public long Position => _BinaryWriter.BaseStream.Position;

    private ILasHeader _header;
    public ILasHeader Header
    {
        get => _header;
        set => _header = value;
    }

    public LasWriter(string lasFile)
    {
        _StreamWriter = new StreamWriter(File.Open(lasFile, FileMode.Create));
        _BinaryWriter = new BinaryWriter(_StreamWriter.BaseStream);
    }

    #region ILasWriter Members
    public long Seek(int position)
    {
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
        if (position > _BinaryWriter.BaseStream.Length) throw new ArgumentOutOfRangeException(nameof(position));

        return _BinaryWriter.Seek(position, SeekOrigin.Begin);
    }

    public void WriteHeader()
    {
        Header.WriteToFile(_BinaryWriter);
    }

    public void WriteHeader(ILasHeader header)
    {
        _header = header;
        WriteHeader();
    }

    public void WriteVLR(ILasVariableLengthRecord vlr)
    {
        if (Position != _header.HeaderSize) Seek(_header.HeaderSize);

        vlr.WriteToFile(_BinaryWriter);
    }

    public void WriteVLRs(IEnumerable<ILasVariableLengthRecord> vlrs)
    {
        if (Position != _header.HeaderSize) Seek(_header.HeaderSize);

        foreach (var vlr in vlrs) vlr.WriteToFile(_BinaryWriter);
    }

    public void WritePoint(ILasPoint point)
    {
        throw new NotImplementedException();
    }

    public void WritePoints(IEnumerable<ILasPoint> points)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(byte[] bytes)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Internal Methods

    #endregion

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing) _StreamWriter.Dispose();

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