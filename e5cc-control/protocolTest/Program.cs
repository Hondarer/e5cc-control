using libE5cc;

namespace ProtocolTest
{
    class ProtocolTest
    {
        static int Main(string[] args)
        {
            E5ccDriver driver = new E5ccDriver();

            //#region EchoBackTest
            //ResponseBase response = driver.SendCommand(new EchobackTestCommand());

            //foreach (byte data in response.Bytes)
            //{
            //    Console.Write($"0x{data:x2} ");
            //}
            //Console.Write("\r\n");
            //#endregion

            #region Read
            ResponseBase response = driver.SendCommand(new ReadVariableMultipleCommand() {ReadStartAddress=0x0000, NumberOfElements=6 });

            foreach (var data in ((ReadVariableMultipleResponse)response).Variables)
            {
                Console.Write($"0x{data.Key:x4}: {data.Value}\r\n");
            }
            Console.Write("\r\n");
            #endregion

            return 0;
        }
    }
}
