using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace libE5cc
{
    public class E5ccDriver : IDisposable
    {
        private readonly SerialPort serialPort;

        private bool disposedValue;

        public E5ccDriver()
        {
            serialPort = new SerialPort()
            {
                PortName = "COM4",
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.Even
            };

            serialPort.Open();
            //Thread.Sleep(5000);

            ResponseBase response;

            // 通信書込: 01：ON（許可）
            response = SendCommand(new OperationCommand() { CommandCode = 0x00, RelatedInformation = 0x01 });

            // 書込モード: 01：RAM 書込モード
            response = SendCommand(new OperationCommand() { CommandCode = 0x04, RelatedInformation = 0x01 });

            // 設定温度変更
            response = SendCommand(new WriteVariableSingleCommand() { WriteVariableAddress = 0x2103, WriteData = 5000 });

            // RAMデータ保存
            //response = SendCommand(new OperationCommand() { CommandCode = 0x05, RelatedInformation = 0x00 });

            foreach (byte data in response.Bytes)
            {
                Debug.Write($"0x{data:x2} ");
            }
            Debug.Write("\r\n");
        }

        public ResponseBase SendCommand(CommandBase commandBase)
        {
            int retryCount = 0;
            while (retryCount < 2)
            {
                try
                {
                    serialPort.Write(commandBase.Bytes, 0, commandBase.Bytes.Length);

                    ResponseBase response = ResponseFactory.GetResponse(serialPort, commandBase);
                    return response;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    serialPort.DiscardInBuffer();
                }

                Thread.Sleep(100);
                retryCount++;
            }

            throw new Exception();
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージド状態を破棄します (マネージド オブジェクト)
                    serialPort.Close();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~E5ccDriver()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
