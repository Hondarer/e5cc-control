using System;
using System.IO.Ports;
using System.Threading;

namespace libE5cc
{
    public class ResponseFactory
    {
        public static ResponseBase GetResponse(SerialPort serialPort, CommandBase commandBase)
        {
            // TODO: タイムアウトの実装などが本来必要
            while (serialPort.BytesToRead < 2)
            {
                Thread.Sleep(100);
            }

            byte[] recvData = new byte[2];
            serialPort.Read(recvData, 0, 2);

            if (recvData[1] == (byte)FunctionCode.WriteVariableMultiple)
            {
                // TODO: タイムアウトの実装などが本来必要
                while (serialPort.BytesToRead < 1)
                {
                    Thread.Sleep(100);
                }

                Array.Resize(ref recvData, 3);
                serialPort.Read(recvData, 2, 1);

                // TODO: タイムアウトの実装などが本来必要
                while (serialPort.BytesToRead < recvData[2])
                {
                    Thread.Sleep(100);
                }

                Array.Resize(ref recvData, recvData.Length + recvData[2] + 2);
                serialPort.Read(recvData, 2, recvData[2] + 2);

                return new ReadVariableMultipleResponse() { Bytes = recvData };
            }

            if (recvData[1] == (byte)FunctionCode.WriteVariableSingleOrOperationCommand)
            {
                // TODO: タイムアウトの実装などが本来必要
                while (serialPort.BytesToRead < 6)
                {
                    Thread.Sleep(100);
                }

                Array.Resize(ref recvData, 8);
                serialPort.Read(recvData, 2, 6);

                if ((recvData[2] == 0x00 && recvData[3] == 0x00) ||
                    (recvData[2] == 0xff && recvData[3] == 0xff))
                {
                    return new OperationResponse() { Bytes = recvData };
                }

                return new WriteVariableSingleResponse() { Bytes = recvData };
            }

            if (recvData[1] == (byte)FunctionCode.EchobackTest)
            {
                // TODO: タイムアウトの実装などが本来必要
                while (serialPort.BytesToRead < 6)
                {
                    Thread.Sleep(100);
                }

                Array.Resize(ref recvData, 8);
                serialPort.Read(recvData, 2, 6);

                return new EchobackTestResponse() { Bytes = recvData };
            }

            // TODO: タイムアウトの実装などが本来必要
            while (serialPort.BytesToRead < 3)
            {
                Thread.Sleep(100);
            }

            Array.Resize(ref recvData, 5);
            serialPort.Read(recvData, 2, 3);

            return new ErrorResponse() { Bytes = recvData };
        }
    }
}
