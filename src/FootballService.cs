using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Android.Net.Http;
using Java.Io;
using Org.Apache.Http.Client.Methods;
using Org.Json;

namespace WillFootballRuinMyDay
{
    public class FootballService
    {
        internal static List<Fixture> GetFixtures(int teamId)
        {
            var content = GetUpcomingFixturesForTeam(teamId);
            if (content == null) return null;
            var jsonArray = new JSONArray(content);
            return ConvertJsonArrayToFixtureList(jsonArray);
        }

        private static List<Fixture> ConvertJsonArrayToFixtureList(JSONArray jsonArray)
        {
            var list = new List<Fixture>();
            var length = jsonArray.Length();
            for (var i = 0; i < length; i++)
            {
                var jsonObject = jsonArray.OptJSONObject(i);

                if (jsonObject == null) continue;


                var fixture = new Fixture
                    {
                        AwayTeam = jsonObject.GetString("awayTeam"),
                        HomeTeam = jsonObject.GetString("homeTeam"),
                        Date = ParseUtcDate(jsonObject.GetString("date"))
                    };
                list.Add(fixture);
            }
            return list;
        }

        private static DateTime ParseUtcDate(string utcDate)
        {
            //convert 2014-08-16T11:45:00Z format into DateTime. Dot42 seems to struggle.
            utcDate = utcDate.Replace("Z", "");

            var date = utcDate.Substring(0, utcDate.IndexOf('T')).Split("-");
            var time = utcDate.Substring(utcDate.IndexOf('T')+1).Split(":");

            return new DateTime(Convert.ToInt16(date[0]),
                Convert.ToInt16(date[1]),
                Convert.ToInt16(date[2]),
                Convert.ToInt16(time[0]),
                Convert.ToInt16(time[1]),
                Convert.ToInt16(time[2]));
        }

        /// <summary>
        /// Perform the low level request and return the resulting content.
        /// </summary>
        private static string GetUpcomingFixturesForTeam(int teamId)
        {
            var uri = string.Format("http://www.football-data.org/team/{0}/fixtures/upcoming", teamId);
            var client = AndroidHttpClient.NewInstance("WillFootballRuinMyDay");
            var request = new HttpGet(uri);
            try
            {
                var response = client.Execute(request);
                var content = response.GetEntity().GetContent();
                var reader = new BufferedReader(new InputStreamReader(content));
                var builder = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    builder.Append(line);
                }
                return builder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
