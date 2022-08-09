using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.Arguments;

namespace SimpleME.Classes
{
    class LogArgument : Argument<string>
    {
        public LogArgument(string value) : base(value)
        {

        }
        protected override string GetStringValue()
        {
            char c = '"';
            return " -progress " + c + @Value + c + " ";
        }
    }
}
