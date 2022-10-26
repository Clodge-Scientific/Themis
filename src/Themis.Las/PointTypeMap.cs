using System.Runtime.InteropServices;

using Themis.Las.Structs;

namespace Themis.Las;

public static class PointTypeMap
{
    public static readonly Dictionary<byte, Type> TypeByPointDataFormat = new();
    public static readonly Dictionary<Type, byte> PointDataFormatByType = new();
    public static readonly Dictionary<Type, ushort> SizeByType = new();

    static PointTypeMap()
    {
        Add<LasPointRecordFormat0>(0);
        Add<LasPointRecordFormat1>(1);
        Add<LasPointRecordFormat2>(2);
        Add<LasPointRecordFormat3>(3);
        Add<LasPointRecordFormat4>(4);
        Add<LasPointRecordFormat5>(5);
        Add<LasPointRecordFormat6>(6);
        Add<LasPointRecordFormat7>(7);
        Add<LasPointRecordFormat8>(8);
        Add<LasPointRecordFormat9>(9);
        Add<LasPointRecordFormat10>(10);
    }

    static void Add<T>(byte pointType)
    {
        TypeByPointDataFormat.Add(pointType, typeof(T));
        PointDataFormatByType.Add(typeof(T), pointType);
        SizeByType.Add(typeof(T), (ushort)Marshal.SizeOf<T>());
    }

    public static byte GetPointRecordTypeByte(Type T)
    {
        if (!PointDataFormatByType.ContainsKey(T)) throw new NotImplementedException($"Unknown PointDataFormat type entered: {T}");

        return PointDataFormatByType[T];
    }

    public static ushort GetPointRecordLength(Type T)
    {
        if (!SizeByType.ContainsKey(T)) throw new NotImplementedException($"Unknown PointDataFormat type entered: {T}");

        return SizeByType[T];
    }

    public static Type GetPointRecordType(ILasHeader header)
    {
        if (!TypeByPointDataFormat.ContainsKey(header.PointDataFormat)) throw new NotImplementedException($"Unknown PointDataRecord format encountered: {header.PointDataFormat} - expected [0, 10]");

        return TypeByPointDataFormat[header.PointDataFormat];
    }

    public static ushort GetPointRecordLength(byte format)
    {
        if (!TypeByPointDataFormat.ContainsKey(format)) throw new NotImplementedException($"Unknown PointDataRecord format encountered: {format} - expected [0, 10]");

        var T = TypeByPointDataFormat[format];
        return GetPointRecordLength(T);
    }
}