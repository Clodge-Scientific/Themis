namespace Themis.Las.Stream;

internal interface IStreamBuffer
{
    /// <summary>
    /// Total number of points 'loaded' into the IStreamBuffer
    /// </summary>
    int Loaded { get; }
    /// <summary>
    /// Total number of points 'consumed' by an IStreamHandler by this IStreamBuffer
    /// </summary>
    int Consumed { get; }
    /// <summary>
    /// The total number of points available for consumption by an IStreamHandler
    /// </summary>
    int Available { get; }

    /// <summary>
    /// Array housing all byte data for the IStreamBuffer
    /// </summary>
    Array Data { get; }

    /// <summary>
    /// Set the current number of loaded points in this IStreamBuffer
    /// </summary>
    /// <param name="num">Desired number to be set</param>
    void SetLoaded(int num);
    /// <summary>
    /// Set the current number of consumed points in this IStreamBuffer
    /// </summary>
    /// <param name="num">Desired number to be set</param>
    void SetConsumed(int num);

    /// <summary>
    /// Read the next point record from memory and return it as a LasPoint
    /// </summary>
    /// <param name="header">ILasHeader to be used to generate the LasPoint</param>
    /// <returns></returns>
    LasPoint GetNext(ILasHeader header);
    /// <summary>
    /// Read the next point record from memory and update the input LasPoint
    /// </summary>
    /// <param name="header">ILasHeader to be used to generate the LasPoint</param>
    /// <param name="lpt">LasPoint to be updated</param>
    void GetNext(ILasHeader header, ref LasPoint lpt);
}