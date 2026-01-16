using System;

namespace VITRACK.Common.Helpers
{
    public static class TimeHelper
    {
        // Azerbaijan is in UTC+4 (AZT) permanently since 2016
        public static DateTime GetBakuTime()
        {
            return DateTime.UtcNow.AddHours(4);
        }
    }
}