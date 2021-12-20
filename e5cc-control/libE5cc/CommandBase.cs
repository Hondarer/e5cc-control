namespace libE5cc
{
    public abstract class CommandBase : FormatBase
    {
        public abstract byte[] Bytes { get; }

        protected ByteMode GetByteMode(ushort address)
        {
            if (address >= 0x2000)
            {
                return ByteMode.TwoBytes;
            }

            return ByteMode.FourBytes;
        }
    }
}
