using System.IO.Ports;

namespace protocolTest
{
    class protocolTest
    {
        static int Main(string[] args)
        {

            SerialPort serialPort = new SerialPort()
            {
                PortName = "COM4",
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.Even
            };

            serialPort.Open();

            EchobackTestCommand echobackTestCommand = new EchobackTestCommand();
            serialPort.Write(echobackTestCommand.Bytes, 0, echobackTestCommand.Bytes.Length);

            while (serialPort.BytesToRead < 8)
            {
                Thread.Sleep(8);
            }

            byte[] recvDataWithCRC = new byte[8];
            serialPort.Read(recvDataWithCRC, 0, 8);

            foreach (byte data in recvDataWithCRC)
            {
                Console.Write($"0x{data:x2} ");
            }
            Console.Write("\r\n");

#if false
            //byte[] sendDataWithCRC = CalculateAndAddCRC(new byte[] { 0x01, 0x08, 0x00, 0x00, 0x12, 0x34 });
            byte[] sendDataWithCRC = CalculateAndAddCRC(new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0c });
            serialPort.Write(sendDataWithCRC, 0, sendDataWithCRC.Length);

            while (serialPort.BytesToRead < 29)
            {
                Thread.Sleep(10);
            }

            byte[] recvDataWithCRC = new byte[29];
            serialPort.Read(recvDataWithCRC, 0, 29);

            foreach (byte data in recvDataWithCRC)
            {
                Console.Write($"0x{data:x2} ");
            }
            Console.Write("\r\n");

            // 設定値変更(あらかじめ通信書込をONにしておく必要がある)

            // 設定値を200.0度に
            sendDataWithCRC = CalculateAndAddCRC(new byte[] { 0x01, 0x10, 0x01, 0x06, 0x00, 0x02, 0x04, 0x00, 0x00, 0x07, 0xd0 });
            serialPort.Write(sendDataWithCRC, 0, sendDataWithCRC.Length);

            // エラーの時は、受信バイト数が少ないので、
            // ファンクションコードを意識したバイト数を処理しないといけない。
            while (serialPort.BytesToRead < 8)
            {
                Thread.Sleep(10);
            }

            recvDataWithCRC = new byte[8];
            serialPort.Read(recvDataWithCRC, 0, 8);

            foreach (byte data in recvDataWithCRC)
            {
                Console.Write($"0x{data:x2} ");
            }
            Console.Write("\r\n");
#endif

            serialPort.Close();

            return 0;
        }
    }
}
