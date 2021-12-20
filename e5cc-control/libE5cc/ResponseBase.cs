using System;
using System.Collections.Generic;
using System.Text;

namespace libE5cc
{
    public abstract class ResponseBase : FormatBase
    {
        public byte[] Bytes { internal set; get; }
    }
}
