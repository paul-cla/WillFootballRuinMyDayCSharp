using System;
using System.Collections.Generic;
using System.Text;
using Android.Util;
using Java.Io;
using Java.Net;
using Org.Json;

namespace WillFootballRuinMyDay
{
    public class FootballService
    {
        internal static List<Fixture> GetFixtures(int teamId, bool forceRefresh = false)
        {
            var content = GetUpcomingFixturesForTeam(teamId, forceRefresh);
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
                        Date = DateHelper.ParseUtcDate(jsonObject.GetString("date"))
                    };
                list.Add(fixture);
            }
            return list;
        }

        private static string GetUpcomingFixturesForTeam(int teamId, bool forceRefresh = false)
        {
            var url = new URL(string.Format("http://www.football-data.org/team/{0}/fixtures/upcoming", teamId));
            var urlConnection = (HttpURLConnection)url.OpenConnection();
            try
            {
                if (forceRefresh)
                {
                    urlConnection.AddRequestProperty("Cache-Control", "no-cache");
                }
                else
                {
                    urlConnection.AddRequestProperty("Cache-Control", "max-stale=" + 86400); //1 day
                }

                urlConnection.SetUseCaches(true);
                var reader = new BufferedReader(new InputStreamReader(urlConnection.GetInputStream()));
                var builder = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    builder.Append(line);
                }
                return builder.ToString();
            }
            catch (Exception ex)
            {
                Log.I(Constants.Tag, "Error getting fixtures. " + ex.Message + " " + ex.StackTrace);
                return null;
            }
        }
    }
}
