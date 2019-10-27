namespace Common.DateTime
{
    using System;

    public static class UnixTime
    {
        private static readonly  DateTime UnixStartTime = new DateTime(1970, 1, 1);
        
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return (long)dateTime.Subtract(UnixStartTime).TotalMilliseconds;
        }

        public static DateTime FromUnixTimestampToDateTime(long dateTimestamp)
        {
            return UnixStartTime.AddMilliseconds(dateTimestamp);
        }
    }
}