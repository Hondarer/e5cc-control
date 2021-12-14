using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocolTest
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
                byte[] bytes = new byte[] { SlaveAddress, (byte)FunctionCode, 0x00, 0x00 };
                bytes = bytes.Concat(BitConverter.GetBytes(WriteVariableAddress).Reverse()).
                              Concat(BitConverter.GetBytes(WriteData).Reverse()).ToArray();
                return bytes.Concat(CalculateCRC(bytes)).ToArray();
            }
            set => throw new InvalidOperationException();
        }
    }
}
