using System;
using System.Text;
using Android.Net.Http;
using Java.Io;
using Org.Apache.Http.Client.Methods;
using Org.Json;

namespace WillFootballRuinMyDay
{
    public class FootballService
    {
        internal static UpcomingFixtures GetFixtures(int teamId)
        {
            var content = GetUpcomingFixturesForTeam(teamId);
            if (content == null) return null;
            else
            {
                JSONArray jsonObject;
                try
                {
                    jsonObject = new JSONArray(content);
                }
                catch (Exception ex)
                {
                    
                    throw;
                }
                
                return new UpcomingFixtures(jsonObject);
            }
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
