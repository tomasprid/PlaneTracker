using System;
using System.Collections.Generic;
using System.Text;

namespace PlaneTracker.Shared
{
    public static class Extensions
    {
        public static DateTime FromUnixTimestamp(this DateTime dateTime, double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double ToUnixTimestamp(this DateTime dateTime)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = dateTime.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
