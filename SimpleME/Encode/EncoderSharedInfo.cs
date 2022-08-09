using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleME.Encode
{
    public class EncoderSharedInfo
    {
        public bool IsEncoding;
        public bool IsConfigured;


        public EncoderSharedInfo()
        {
            IsEncoding = false;
            IsConfigured = false;
        }
    }
}
