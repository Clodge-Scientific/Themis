namespace Themis.Las;

public static class Constants
{
    public const int LasHeader12Size = 227;
    public const int LasHeader14Size = 375;
    public const int LasHeaderSignature = 1179861324;

    public const uint LasHeaderVersionMajorOffset = 24;
    public const uint LasHeaderVersionMinorOffset = 25;

    public const char DefaultChar = '\0';
    public const double DefaultScale = 0.01;
    public const double DefaultOffset = 0.0;

    public const uint DefaultReaderBufferCount = 250000;
}