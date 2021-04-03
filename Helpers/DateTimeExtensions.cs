using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Helpers
{
    public static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime date)
        {
            return (int)Math.Ceiling(date.Day / 7.0);
        }
    }
}
