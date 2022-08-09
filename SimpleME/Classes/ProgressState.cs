using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SimpleME.Classes
{
    public sealed class ProgressState
    {
        public float MaxValue { get; private set; }
        private volatile float _value;

        public ProgressState() : this(100)
        {
            _value = 0;
        }
        public ProgressState(float maxValue)
        {
            MaxValue = maxValue;
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                var last = _value;
                _value = value;
                if (last < MaxValue)
                    this.OnProgress(new ProgressChangedEventArgs((int)(_value / MaxValue * 100), this));
                else
                    this.OnRegress(new ProgressChangedEventArgs((int)(_value / MaxValue * 100), this));
            }
        }

        public event ProgressChangedEventHandler Progress;
        private void OnProgress(ProgressChangedEventArgs e)
        {
            Progress?.Invoke(this, e);
        }

        public event ProgressChangedEventHandler Regress;
        private void OnRegress(ProgressChangedEventArgs e)
        {
            Regress?.Invoke(this, e);
        }

    }
}
