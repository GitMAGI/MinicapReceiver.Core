using System;
using System.Collections.Generic;
using System.Text;

namespace CA_Main
{
    public static class Utils
    {
        public static string ElapsedTime(TimeSpan ts)
        {
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000} (hh:mm:ss.mmm)", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            return elapsedTime;
        }
    }
}
