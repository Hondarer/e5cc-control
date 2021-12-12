using System.IO.Ports;

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

        byte[] sendDataWithCRC = CalculateAndAddCRC(new byte[] { 0x01, 0x08, 0x00, 0x00, 0x12, 0x34 });
        serialPort.Write(sendDataWithCRC, 0, sendDataWithCRC.Length);

        while (serialPort.BytesToRead < 8)
        {
            Thread.Sleep(10);
        }

        byte[] recvDataWithCRC = new byte[8];
        serialPort.Read(recvDataWithCRC, 0, 8);

        foreach (byte data in recvDataWithCRC)
        {
            Console.Write($"0x{data:x2} ");
        }
        Console.Write("\r\n");

        serialPort.Close();

        return 0;
    }

    static byte[] CalculateAndAddCRC(byte[] buffer)
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

        Array.Resize(ref buffer, buffer.Length + 2);
        buffer[^2] = (byte)(crc & 0xff);
        buffer[^1] = (byte)(crc >> 8 & 0xff);

        return buffer;
    }
}
