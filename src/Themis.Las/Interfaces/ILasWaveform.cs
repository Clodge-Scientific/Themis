namespace Themis.Las;

/// <summary>
/// Point that contains (spooky) wave packet data
/// </summary>
public interface ILasWaveform
{
    byte WavePacketDescriptorIndex { get; }

    ulong ByteOffsetToWaveformData { get; }

    uint WaveformPacketSizeBytes { get; }

    float ReturnPointWaveformLocation { get; }
    float X_t { get; }
    float Y_t { get; }
    float Z_t { get; }
}