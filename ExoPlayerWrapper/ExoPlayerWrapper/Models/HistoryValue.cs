using System;

namespace ExoPlayerWrapper.Models
{
    public class HistoryValue<T> where T : struct
    {
        public HistoryValue(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
    }
}