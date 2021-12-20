using System;
using System.IO.Ports;

namespace libE5cc
{
    public class E5ccDriver : IDisposable
    {
        private SerialPort serialPort;

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
        }

        public ResponseBase SendCommand(CommandBase commandBase)
        {
            serialPort.Write(commandBase.Bytes, 0, commandBase.Bytes.Length);

            return ResponseFactory.GetResponse(serialPort, commandBase);
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
