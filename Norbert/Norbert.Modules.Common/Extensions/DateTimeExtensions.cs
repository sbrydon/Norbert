using System;

namespace Norbert.Modules.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime dateTime)
        {
            return (dateTime.Ticks - 621355968000000000)/10000000;
        }
    }
}