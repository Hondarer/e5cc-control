namespace libE5cc
{
    public class OperationCommand : WriteVariableSingleCommand
    {
        private byte _commandCode = 0x00;

        public byte CommandCode
        {
            get => _commandCode;
            set
            {
                _commandCode = value;
                WriteVariableAddress = 0x0000;
                WriteData &= 0x00ff;
                WriteData |= (ushort)(value << 8);
            }
        }

        private byte _relatedInformation = 0x00;

        public byte RelatedInformation
        {
            get => _relatedInformation;
            set
            {
                _relatedInformation = value;
                WriteVariableAddress = 0x0000;
                WriteData &= 0xff00;
                WriteData |= value;
            }
        }
    }
}
