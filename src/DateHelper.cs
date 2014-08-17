using System;

namespace WillFootballRuinMyDay
{
    public class DateHelper
    {
        public static DateTime ParseUtcDate(string utcDate)
        {
            //convert 2014-08-16T11:45:00Z format into DateTime. Dot42 seems to struggle.
            utcDate = utcDate.Replace("Z", "");

            var date = utcDate.Substring(0, utcDate.IndexOf('T')).Split("-");
            var time = utcDate.Substring(utcDate.IndexOf('T') + 1).Split(":");

            return new DateTime(Convert.ToInt16(date[0]),
                                Convert.ToInt16(date[1]),
                                Convert.ToInt16(date[2]),
                                Convert.ToInt16(time[0]),
                                Convert.ToInt16(time[1]),
                                Convert.ToInt16(time[2]));
        }
    }
}