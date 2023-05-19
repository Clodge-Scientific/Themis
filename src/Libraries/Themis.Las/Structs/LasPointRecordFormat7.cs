using System.Runtime.InteropServices;

namespace Themis.Las.Structs
{
    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct LasPointRecordFormat7 : ILasPointStruct, ILasTime, ILasRgb
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
        private ushort _GlobalEncoding;

        [FieldOffset(16)]
        private byte _Classification;
        [FieldOffset(17)]
        private byte _UserData;
        [FieldOffset(18)]
        private short _ScanAngle;

        [FieldOffset(20)]
        private ushort _FlightLine;

        [FieldOffset(22)]
        private double _Timestamp;

        [FieldOffset(30)]
        private ushort _R;
        [FieldOffset(32)]
        private ushort _G;
        [FieldOffset(34)]
        private ushort _B;
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
            set => _GlobalEncoding = value;
        }
        public short ScanAngle
        {
            get => _ScanAngle;
            set => _ScanAngle = value;
        }
        public double Timestamp
        {
            get => _Timestamp;
            set => _Timestamp = value;
        }
        public ushort R
        {
            get => _R;
            set => _R = value;
        }
        public ushort G
        {
            get => _G;
            set => _G = value;
        }
        public ushort B
        {
            get => _B;
            set => _B = value;
        }
        #endregion

        public static LasPointRecordFormat7 GetLasPointStruct(LasPoint lpt, ILasHeader header)
        {
            return new LasPointRecordFormat7
            {
                _X = LasPoint.GetIntegerPosition(lpt.X, header),
                _Y = LasPoint.GetIntegerPosition(lpt.Y, header),
                _Z = LasPoint.GetIntegerPosition(lpt.Z, header),
                _Intensity = lpt.Intensity,
                _GlobalEncoding = lpt.GlobalEncoding,
                _Classification = lpt.Classification,
                _UserData = lpt.UserData,
                _ScanAngle = lpt.ScanAngle,
                _FlightLine = lpt.FlightLine,
                _Timestamp = lpt.Timestamp,
                _R = lpt.R,
                _G = lpt.G,
                _B = lpt.B
            };
        }
    }
}
