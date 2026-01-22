using System;

namespace VITRACK.Common.Helpers
{
    public static class TimeHelper
    {
        // Azerbaijan is in UTC+4 (AZT) permanently since 2016
        private const int BakuUtcOffsetHours = 4;
        public static DateTime GetBakuTime()
        {
            return DateTime.UtcNow.AddHours(BakuUtcOffsetHours);
        }

        public static DateOnly GetBakuDate()
        {
            return DateOnly.FromDateTime(GetBakuTime());
        }

        public static TimeOnly GetBakuTimeOnly()
        {
            return TimeOnly.FromDateTime(GetBakuTime());
        }
    }
}