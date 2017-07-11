using System;
using Google.Apis.Calendar.v3.Data;

namespace EmailParserForCalendar
{
    public static class Extensions
    {
        public static DateTime? FromEpochMs(this long? time)
        {
            if (!time.HasValue) return null;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(time.Value).ToLocalTime();
        }

        public static EventDateTime ToEventDateTime(this DateTimeOffset value)
        {
            return new EventDateTime
            {
                DateTime = value.DateTime,
                TimeZone = String.Format("{0:zzz}", value)
            };
        }
    }
}