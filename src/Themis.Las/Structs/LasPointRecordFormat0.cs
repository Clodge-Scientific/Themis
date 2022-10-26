using System.Runtime.InteropServices;

namespace Themis.Las.Structs
{
    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public struct LasPointRecordFormat0 : ILasPointStruct
    {
        #region Private Fields
        [FieldOffset(4 * 0)]
        private int _X;
        [FieldOffset(4 * 1)]
        private int _Y;
        [FieldOffset(4 * 2)]
        private int _Z;

        [FieldOffset(4 * 3)]
        private ushort _Intensity;

        [FieldOffset(14)]
        private byte _GlobalEncoding;
        [FieldOffset(15)]
        private byte _Classification;
        [FieldOffset(16)]
        private byte _ScanAngle;
        [FieldOffset(17)]
        private byte _UserData;

        [FieldOffset(18)]
        private ushort _FlightLine;
        #endregion

        #region Public Fields
        public int X
        {
            get => _X;
            set => _X = value;
        }
        public int Y
        {
            get => _Y;
            set => _Y = value;
        }
        public int Z
        {
            get => _Z;
            set => _Z = value;
        }

        public byte Classification
        {
            get => _Classification;
            set => _Classification = value;
        }
        public byte UserData
        {
            get => _UserData;
            set => _UserData = value;
        }

        public ushort Intensity
        {
            get => _Intensity;
            set => _Intensity = value;
        }
        public ushort FlightLine
        {
            get => _FlightLine;
            set => _FlightLine = value;
        }
        public ushort GlobalEncoding
        {
            get => _GlobalEncoding;
            set => _GlobalEncoding = (byte)value;
        }

        public short ScanAngle
        {
            get => _ScanAngle;
            set => _ScanAngle = (byte)value;
        }
        #endregion

        public static LasPointRecordFormat0 GetLasPointStruct(LasPoint lpt, ILasHeader header)
        {
            return new LasPointRecordFormat0
            {
                _X = LasPoint.GetIntegerPosition(lpt.X, header),
                _Y = LasPoint.GetIntegerPosition(lpt.Y, header),
                _Z = LasPoint.GetIntegerPosition(lpt.Z, header),
                _Intensity = lpt.Intensity,
                _GlobalEncoding = (byte)lpt.GlobalEncoding,
                _Classification = lpt.Classification,
                _ScanAngle = (byte)lpt.ScanAngle,
                _UserData = lpt.UserData,
                _FlightLine = lpt.FlightLine,
            };
        }
    }
}
