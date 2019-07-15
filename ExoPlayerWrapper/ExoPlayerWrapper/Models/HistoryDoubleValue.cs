using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlayerWrapper.Models
{
    public class HistoryDoubleValue : HistoryValue<double>
    {
        public HistoryDoubleValue(double oldValue, double newValue) : base(oldValue, newValue)
        {
        }

        public bool IsValid => OldValue > 0 && NewValue > 0;
    }
}
