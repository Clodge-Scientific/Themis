using Themis.Las.Time;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Las;

public class LasPoint : ILasPoint
{
    public byte UserData { get; set; }
    public byte Classification { get; set; }

    public ushort Intensity { get; set; }
    public ushort FlightLine { get; set; }
    public ushort GlobalEncoding { get; set; }

    public short ScanAngle { get; set; }

    public double X => Position[0];
    public double Y => Position[1];
    public double Z => Position[2];
    public Vector<double> Position { get; set; } = CreateVector.Dense(new double[] { double.NaN, double.NaN, double.NaN });

    public double Timestamp { get; set; }
    public DateTime DateTime => GpsTime.Parse(Timestamp);

    public ushort R { get; set; }
    public ushort G { get; set; }
    public ushort B { get; set; }
    public ushort NIR { get; set; }

    public byte WavePacketDescriptorIndex { get; set; }
    public uint WaveformPacketSizeBytes { get; set; }

    public float ReturnPointWaveformLocation { get; set; }
    public float X_t { get; set; }
    public float Y_t { get; set; }
    public float Z_t { get; set; }

    public ulong ByteOffsetToWaveformData { get; set; }

    public void Update(ILasTime time)
    {
        this.Timestamp = time.Timestamp;
    }

    public void Update(ILasRgb p)
    {
        this.R = p.R;
        this.G = p.G;
        this.B = p.B;

        if (p is ILas4Band band) this.NIR = band.NIR;
    }

    public void Update(ILasWaveform p)
    {
        this.WaveformPacketSizeBytes = p.WaveformPacketSizeBytes;
        this.ByteOffsetToWaveformData = p.ByteOffsetToWaveformData;
        this.WavePacketDescriptorIndex = p.WavePacketDescriptorIndex;

        this.ReturnPointWaveformLocation = p.ReturnPointWaveformLocation;
        this.X_t = p.X_t;
        this.Y_t = p.Y_t;
        this.Z_t = p.Z_t;
    }

    public void Update(ILasPointStruct p, ILasHeader header)
    {
        this.Position[0] = p.X * header.ScaleX + header.OriginX;
        this.Position[1] = p.Y * header.ScaleY + header.OriginY;
        this.Position[2] = p.Z * header.ScaleZ + header.OriginZ;

        this.ScanAngle = p.ScanAngle;
        this.Intensity = p.Intensity;
        this.FlightLine = p.FlightLine;
        this.GlobalEncoding = p.GlobalEncoding;
        this.Classification = p.Classification;
    }

    public static int GetIntegerPosition(double pos, ILasHeader header)
    {
        return (int)((pos - header.OriginX) / header.ScaleX);
    }
}