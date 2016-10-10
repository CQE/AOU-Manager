using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    class AOUHelper
    {

        public static uint GetNowToMs()
        {
            var now = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, now.Hour, now.Minute, now.Second);

            return (uint)ts.TotalMilliseconds;
        }

        public static long ToCurTimeStep(long ms, double msbetween)
        {
            return (long)(Math.Round(ms * 1.0 / msbetween) * msbetween);

        }

        public static TimeSpan msToTimeSpan(long time_ms)
        {
            return TimeSpan.FromMilliseconds(time_ms);
        }

        public static string msToTimeSpanStr(long time_ms)
        {
            string timestr = "";
            try
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(time_ms * 1.0);
                if (ts.Days > 0) timestr += ts.Days + ".";
                timestr += ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
                int ds = (int)Math.Round(ts.Milliseconds / 100.0); // deci sek > 0 ?
                if (ds > 0)
                {
                    timestr += "." + ds; 
                }
            }
            catch (Exception e)
            {
                timestr = e.Message;
            }
            return timestr;
        }

    }
}
