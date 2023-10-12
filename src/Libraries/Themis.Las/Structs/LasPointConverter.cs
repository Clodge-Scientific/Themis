namespace Themis.Las.Structs;

public static class LasPointConverter
{
    public static ILasPointStruct GetLasPointStruct(LasPoint lpt, ILasHeader header)
    {
        switch (header.PointDataFormat)
        {
            case 0:
                return LasPointRecordFormat0.GetLasPointStruct(lpt, header);
            case 1:
                return LasPointRecordFormat1.GetLasPointStruct(lpt, header);
            case 2:
                return LasPointRecordFormat2.GetLasPointStruct(lpt, header);
            case 3:
                return LasPointRecordFormat3.GetLasPointStruct(lpt, header);
            case 4:
                return LasPointRecordFormat4.GetLasPointStruct(lpt, header);
            case 5:
                return LasPointRecordFormat5.GetLasPointStruct(lpt, header);
            case 6:
                return LasPointRecordFormat6.GetLasPointStruct(lpt, header);
            case 7:
                return LasPointRecordFormat7.GetLasPointStruct(lpt, header);
            case 8:
                return LasPointRecordFormat8.GetLasPointStruct(lpt, header);
            case 9:
                return LasPointRecordFormat9.GetLasPointStruct(lpt, header);
            case 10:
                return LasPointRecordFormat10.GetLasPointStruct(lpt, header);
            default:
                throw new Exception($"Attempted to convert to unsupported PointRecordFormat: {header.PointDataFormat}");
        }
    }
}