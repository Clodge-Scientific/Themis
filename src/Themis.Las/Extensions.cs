using System;
using System.IO;
using System.Linq;

namespace Themis.Las;

public static class Extensions
{
    public static T[] PadRight<T>(this T[] arr, int length, T fallback = default(T))
    {
        T[] ret = new T[length];
        foreach (int i in Enumerable.Range(0, length))
        {
            if (i <= length) ret[i] = arr[i];
            else ret[i] = fallback;
        }
        return ret;
    }

    public static void WriteToFile(this ILasVariableLengthRecord vlr, BinaryWriter writer)
    {
        writer.Write(vlr.Reserved);
        writer.Write(vlr.UserID);
        writer.Write(vlr.RecordID);
        writer.Write(vlr.RecordLengthAfterHeader);
        writer.Write(vlr.Description);
        writer.Write(vlr.Data);
    }

    public static void WriteToFile(this ILasHeader header, BinaryWriter writer)
    {
        writer.Write(header.FileSignature);
        writer.Write(header.FileSourceID);
        writer.Write(header.GlobalEncoding);
        writer.Write(header.Guid1);
        writer.Write(header.Guid2);
        writer.Write(header.Guid3);
        writer.Write(header.Guid4);
        writer.Write(header.VersionMajor);
        writer.Write(header.VersionMinor);
        writer.Write(header.SystemIdentifier);
        writer.Write(header.GeneratingSoftware);
        writer.Write(header.CreationDOY);
        writer.Write(header.CreationYear);
        writer.Write(header.HeaderSize);
        writer.Write(header.OffsetToPointData);
        writer.Write(header.NumberOfVLRs);
        writer.Write(header.PointDataFormat);
        writer.Write(header.PointDataRecordLength);
        writer.Write(header.LegacyNumPointRecords);
        for (int i = 0; i < header.LegacyNumPointRecordsByReturn.Length; i++)
        {
            writer.Write(header.LegacyNumPointRecordsByReturn[i]);
        }
        writer.Write(header.ScaleX);
        writer.Write(header.ScaleY);
        writer.Write(header.ScaleZ);
        writer.Write(header.OriginX);
        writer.Write(header.OriginY);
        writer.Write(header.OriginZ);
        writer.Write(header.MaxX);
        writer.Write(header.MinX);
        writer.Write(header.MaxY);
        writer.Write(header.MinY);
        writer.Write(header.MaxZ);
        writer.Write(header.MinZ);

        if (header.VersionMinor == 4)
        {
            writer.Write(header.StartOfWaveformDataPacketRecord);
            writer.Write(header.StartOfFirstExtendedVLR);
            writer.Write(header.NumExtendedVLRs);
            writer.Write(header.NumPointRecords);
            for (int i = 0; i < header.NumPointRecordsByReturn.Length; i++)
            {
                writer.Write(header.NumPointRecordsByReturn[i]);
            }
        }
    }
}