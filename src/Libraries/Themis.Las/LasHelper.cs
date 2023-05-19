using System.Collections;

namespace Themis.Las;

public static class LasHelper
{
    /// <summary>
    /// Generate the GlobalEncoding flag to be used in an ILasHeader
    /// </summary>
    /// <param name="useGpsStandardTime">Boolean flag indicating if we should use GPS Standard time (true) or Seconds of Week (false)</param>
    /// <param name="useProjWkt">(LAS1.4 ONLY) - Boolean flag indicating if the LAS file's CRS is encoded as a Projection WKT VLR</param>
    /// <returns></returns>
    public static ushort GetGlobalEncoding(bool useGpsStandardTime = true, bool useProjWkt = false)
    {
        var newBits = new BitArray(16);

        //< Set the time flag (true being GPS Standard Time, false being Seconds of Week)
        newBits[0] = useGpsStandardTime;
        newBits[1] = false;
        newBits[2] = false;
        newBits[3] = false;
        //< Set the Projection to WKT
        newBits[4] = useProjWkt;
        //< All 'Reserved' bits set to zero
        newBits[5] = false;
        newBits[6] = false;
        newBits[7] = false;
        newBits[8] = false;
        newBits[9] = false;
        newBits[10] = false;
        newBits[11] = false;
        newBits[12] = false;
        newBits[13] = false;
        newBits[14] = false;
        newBits[15] = false;

        var bytes = new byte[2];
        newBits.CopyTo(bytes, 0);

        var value = BitConverter.ToUInt16(bytes, 0);
        return value;
    }
}