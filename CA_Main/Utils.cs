using System;
using System.Collections.Generic;
using System.Text;

namespace CA_Main
{
    public static class Utils
    {
        public static string ElapsedTime(TimeSpan ts)
        {
            if (ts == null)
                return "--unknown--";

            int microseconds = (int)((ts.Ticks / (TimeSpan.TicksPerMillisecond / 1000)) % 1000);
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}{4:000} (hh:mm:ss.mmmuuu)", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, microseconds);
            return elapsedTime;
        }
    }
}
