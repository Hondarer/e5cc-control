namespace libE5cc
{
    public enum FunctionCode : byte
    {
        ReadVariableMultiple = 0x03,
        WriteVariableMultiple = 0x16,
        WriteVariableSingleOrOperationCommand = 0x06,
        EchobackTest = 0x08,

        ErrorResponseReadVariableMultiple = 0x83,
        ErrorResponseWriteVariableMultiple = 0x90,
        ErrorResponseWriteVariableSingleOrOperationCommand = 0x86,
        ErrorResponseEchobackTest = 0x88
    }
}
