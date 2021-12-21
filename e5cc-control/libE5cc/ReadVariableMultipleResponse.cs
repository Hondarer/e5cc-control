using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libE5cc
{
    public class ReadVariableMultipleResponse : ResponseBase
    {
        public Dictionary<ushort, int> Variables { get; private set; } = new Dictionary<ushort, int>();

        public override byte[] Bytes
        {
            internal set
            {
                _bytes = value;

                ByteMode byteMode = GetByteMode(BitConverter.ToUInt16(Command.Bytes.Skip(2).Take(2).Reverse().ToArray()));

                int NumberOfElements = _bytes[2] / (int)byteMode;
                
                Variables.Clear();
                for (int elementNumber=0; elementNumber<NumberOfElements; elementNumber++)
                {
                    int elementValue = BitConverter.ToInt32(_bytes.Skip(3 + elementNumber * (int)byteMode).Take((int)byteMode).Reverse().ToArray());
                    Variables.Add((ushort)(((ReadVariableMultipleCommand)Command).ReadStartAddress + elementNumber * (int)byteMode / 2), elementValue);
                }
            }
        }
    }
}