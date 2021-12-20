using System;
using System.Linq;

namespace libE5cc
{
    public class ReadVariableMultipleCommand : CommandBase
    {
        public ushort ReadStartAddress { get; set; } = 0x0000;

        public ushort NumberOfElements { get; set; } = 0x0001;

        public ReadVariableMultipleCommand()
        {
            FunctionCode = FunctionCode.ReadVariableMultiple;
        }

        public override byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[] { SlaveAddress, (byte)FunctionCode, 0x00, 0x00 };
                ushort multiply = 1;
                if (GetByteMode(ReadStartAddress) == ByteMode.FourBytes)
                {
                    multiply = 2;
                }
                bytes = bytes.Concat(BitConverter.GetBytes(ReadStartAddress).Reverse()).
                              Concat(BitConverter.GetBytes(NumberOfElements * multiply).Reverse()).ToArray();
                return bytes.Concat(CalculateCRC(bytes)).ToArray();
            }
        }
    }
}
