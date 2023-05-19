namespace Themis.Las;

/// <summary>
/// Represents the fundamental fields required for all LAS point (struct, in-memory, or otherwise)
/// </summary>
public interface ILasPointBase
{
    /// <summary>
    /// Full-byte classification flag (rather than just using the first 5bits)
    /// </summary>
    byte Classification { get; }
    /// <summary>
    /// UserData flag, my dude(s)
    /// </summary>
    byte UserData { get; }

    /// <summary>
    /// Intensity of return in range of [0, 65535]
    /// </summary>
    ushort Intensity { get; }
    /// <summary>
    /// Flightline of return in range of [0, 65535]
    /// </summary>
    ushort FlightLine { get; }
    /// <summary>
    /// GlobalEncoding is a 2byte unsigned short in LAS1.4 and a byte previously - we use the larger
    /// </summary>
    ushort GlobalEncoding { get; }

    /// <summary>
    /// ScanAngle is a 2byte short over a single byte - we use the larger
    /// </summary>
    short ScanAngle { get; }
}