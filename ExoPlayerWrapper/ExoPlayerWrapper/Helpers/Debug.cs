using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlayerWrapper.Helpers
{
    public static class Debug
    {
        public static void Log(string message, params object[] args)
        {
            var dateTime = DateTime.Now;
            message = string.Format("{0} -> {1}", dateTime, message);
            System.Diagnostics.Debug.WriteLine(message, args);
        }
    }
}
