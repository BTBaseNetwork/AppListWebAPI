using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutCommon.Utils
{
    public class DateTimeUtil
    {

        public static double UnixTimeSpanSec { get { return UnixTimeSpan.TotalSeconds; } }

        public static TimeSpan UnixTimeSpan
        {
            get
            {
                return UnixTimeSpanOfDateTime(DateTime.UtcNow);
            }
        }

        public static DateTime UnixTimeSpanZeroDate()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0);
        }

        public static TimeSpan UnixTimeSpanOfDateTime(DateTime date)
        {
            return date - UnixTimeSpanZeroDate();
        }

        public static long UnixTimeSpanOfDateTimeMs(DateTime date)
        {
            return (long)UnixTimeSpanOfDateTime(date).TotalMilliseconds;
        }

        public static string ToAccurateDateTimeString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string ToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime ToDate(string dateString)
        {
            try
            {
                var time = DateTime.Parse(dateString);
                time = new DateTime(time.Ticks, DateTimeKind.Utc);
                return time;
            }
            catch (Exception)
            {
                return new DateTime(0);
            }

        }
    }
}
