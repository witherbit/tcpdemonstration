using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Utils
{
    public static class Extensions
    {
        public static byte[] FromUTF8(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public static string ToUTF8(this byte[] arr)
        {
            return Encoding.UTF8.GetString(arr);
        }
        public static byte[] FromBase64String(this string str)
        {
            return Convert.FromBase64String(str);
        }
        public static string ToBase64String(this byte[] arr)
        {
            return Convert.ToBase64String(arr);
        }

        public static string ToSqlDatetime(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        public static DateTime ParseSqlDatetime(this string sqlDatetime)
        {
            return DateTime.Parse(sqlDatetime);
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }
        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - epoch).TotalSeconds;
        }

        public static string ToQuotes(this string str)
        {
            return $"'{str}'";
        }

        public static int GetInt32(this List<object> list, int i)
        {
            return Convert.ToInt32(list[i]);
        }

        public static string GetString(this List<object> list, int i)
        {
            return Convert.ToString(list[i]);
        }

        public static long GetInt64(this List<object> list, int i)
        {
            return Convert.ToInt64(list[i]);
        }

        public static short GetInt16(this List<object> list, int i)
        {
            return Convert.ToInt16(list[i]);
        }
    }
}
