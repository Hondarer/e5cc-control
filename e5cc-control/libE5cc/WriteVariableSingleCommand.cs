using System;
using System.Linq;

namespace libE5cc
{
    public class WriteVariableSingleCommand : CommandBase
    {
        public ushort WriteVariableAddress { get; set; } = 0x0000;

        public ushort WriteData { get; set; } = 0x0000;

        public WriteVariableSingleCommand()
        {
            FunctionCode = FunctionCode.WriteVariableSingleOrOperationCommand;
        }

        public override byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[] { SlaveAddress, (byte)FunctionCode };
                bytes = bytes.Concat(BitConverter.GetBytes(WriteVariableAddress).Reverse()).
                              Concat(BitConverter.GetBytes(WriteData).Reverse()).ToArray();
                return bytes.Concat(CalculateCRC(bytes)).ToArray();
            }
        }
    }
}
