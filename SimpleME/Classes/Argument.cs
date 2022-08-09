using FFMpegCore.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleME.Classes
{
    internal abstract class Argument<T> : IArgument
    {
        protected T Value { get; set; }
        public Argument(T value)
        {
            this.Value = value;
        }
        public string Text => GetStringValue();
        protected abstract string GetStringValue();

    }
}
