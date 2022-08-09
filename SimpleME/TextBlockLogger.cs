using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SimpleME
{
    public class TextBlockLogger : ILogger
    {
        public TextBlock OutputBlock { get; set; }
        public TextBlockLogger(TextBlock outputLabel)
        {
            this.OutputBlock = outputLabel;
        }
        public void Log(LogLevel loglevel, string message, params object[] parameters)
        {
            this.OutputBlock.Text += $"{loglevel}: {string.Format(message, parameters)} \n\n";
        }
    }
}
