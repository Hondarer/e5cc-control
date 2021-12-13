using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocolTest
{
    public abstract class FormatBase
    {
        public byte SlaveAddress { get; set; }

        public FunctionCode FunctionCode { get; protected set; }

        public abstract byte[] Bytes { get; set; }

        protected byte[] CalculateCRC(byte[] buffer)
        {
            ushort crc = 0xffff;

            for (int i = 0; i < buffer.Length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    bool isCarray = (crc & 1) == 1;
                    crc = (ushort)(crc >> 1);
                    if (isCarray)
                    {
                        crc ^= 0xa001;
                    }
                }
            }

            byte[] crcArray = BitConverter.GetBytes(crc);

            // CRC はリトルエンディアンなので、ビッグエンディアンシステムの場合は反転させる
            if (BitConverter.IsLittleEndian != true)
            {
                crcArray = crcArray.Reverse().ToArray();
            }

            return crcArray;
        }
    }
}
