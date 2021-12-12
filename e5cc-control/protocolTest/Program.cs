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

        serialPort.Write(new byte[] { 0x01, 0x08, 0x00, 0x00, 0x12, 0x34, 0xed, 0x7c }, 0, 8);

        while (serialPort.BytesToRead < 8)
        {
            Thread.Sleep(10);
        }

        byte[] recieved = new byte[8];
        serialPort.Read(recieved, 0, 8);

        foreach (byte data in recieved)
        {
            Console.Write($"0x{data:x2} ");
        }
        Console.Write("\r\n");

        serialPort.Close();

        return 0;
    }
}
