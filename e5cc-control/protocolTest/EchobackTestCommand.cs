namespace protocolTest
{
    public class EchobackTestCommand : CommandBase
    {
        public ushort TestData { get; set; } = 0x0000;

        public EchobackTestCommand()
        {
            FunctionCode = FunctionCode.EchobackTest;
        }

        public override byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[] { SlaveAddress, (byte)FunctionCode, 0x00, 0x00 };
                bytes = bytes.Concat(BitConverter.GetBytes(TestData).Reverse()).ToArray();
                return bytes.Concat(CalculateCRC(bytes)).ToArray();
            }
            set => throw new InvalidOperationException();
        }
    }
}
