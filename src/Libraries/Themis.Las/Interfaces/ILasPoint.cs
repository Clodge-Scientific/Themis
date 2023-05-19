namespace Themis.Las;

/// <summary>
/// Represents an in-memory LAS point that exposes the actual position. a timestamp, and all other available fields
/// </summary>
public interface ILasPoint : IPosition, ILasPointBase, ILasTime, ILasRgb, ILas4Band, ILasWaveform
{
    void Update(ILasTime time);
    void Update(ILasRgb rgb);
    void Update(ILasWaveform waveform);
    void Update(ILasPointStruct point, ILasHeader header);
}