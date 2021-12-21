using System;
using System.IO.Ports;
using System.Threading;

namespace libE5cc
{
    public class ResponseFactory
    {
        public static ResponseBase GetResponse(SerialPort serialPort, CommandBase command)
        {
            int timeout = 0;
            while (timeout < 3)
            {
                if (serialPort.BytesToRead >= 2)
                {
                    break;
                }
                Thread.Sleep(100);
                timeout++;
            }
            if (timeout == 3)
            {
                throw new Exception("timeout");
            }

            byte[] recvData = new byte[2];
            serialPort.Read(recvData, 0, 2);

            if (recvData[1] == (byte)FunctionCode.ReadVariableMultiple)
            {
                timeout = 0;
                while (timeout < 3)
                {
                    if (serialPort.BytesToRead >= 1)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeout++;
                }
                if (timeout == 3)
                {
                    throw new Exception("timeout");
                }

                Array.Resize(ref recvData, 3);
                serialPort.Read(recvData, 2, 1);

                timeout = 0;
                while (timeout < 3)
                {
                    if (serialPort.BytesToRead >= (recvData[2] + 2))
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeout++;
                }
                if (timeout == 3)
                {
                    throw new Exception("timeout");
                }

                Array.Resize(ref recvData, recvData.Length + recvData[2] + 2);
                serialPort.Read(recvData, 3, recvData[2] + 2);

                return new ReadVariableMultipleResponse() { Command = command, Bytes = recvData };
            }

            if (recvData[1] == (byte)FunctionCode.WriteVariableSingleOrOperationCommand)
            {
                timeout = 0;
                while (timeout < 3)
                {
                    if (serialPort.BytesToRead >= 6)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeout++;
                }
                if (timeout == 3)
                {
                    throw new Exception("timeout");
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
                timeout = 0;
                while (timeout < 3)
                {
                    if (serialPort.BytesToRead >= 6)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeout++;
                }
                if (timeout == 3)
                {
                    throw new Exception("timeout");
                }

                Array.Resize(ref recvData, 8);
                serialPort.Read(recvData, 2, 6);

                return new EchobackTestResponse() { Bytes = recvData };
            }

            timeout = 0;
            while (timeout < 3)
            {
                if (serialPort.BytesToRead >= 3)
                {
                    break;
                }
                Thread.Sleep(100);
                timeout++;
            }
            if (timeout == 3)
            {
                throw new Exception("timeout");
            }

            Array.Resize(ref recvData, 5);
            serialPort.Read(recvData, 2, 3);

            return new ErrorResponse() { Bytes = recvData };
        }
    }
}
