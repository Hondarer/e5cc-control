using System;
using System.Collections.Generic;
using System.Text;

namespace libE5cc
{
    public abstract class ResponseBase : FormatBase
    {
        public CommandBase? Command { get;internal set; }

        protected byte[] _bytes = new byte[0];

        public virtual byte[] Bytes
        {
            get => _bytes;
            internal set => _bytes = value;
        }
    }
}
