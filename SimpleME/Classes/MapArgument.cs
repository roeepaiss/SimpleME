using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.Arguments;

namespace SimpleME.Classes
{
    class MapArgument : Argument<string[]>
    {
        public MapArgument(params string[] value) : base(value)
        {

        }
        protected override string GetStringValue()
        {
            if (Value.Length == 0)
                return string.Empty;
            char c = '"';
            StringBuilder s = new StringBuilder();
            foreach (var a in Value)
            {
                s.Append("-map " + c + a + "?" + c + " ");
            }
            return s.ToString();
        }
    }
}
