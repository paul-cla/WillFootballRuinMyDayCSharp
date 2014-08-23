using System;

namespace WillFootballRuinMyDay
{
    public class Fixture
    {
        public DateTime Date { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(HomeTeam)) return Date.ToString("dd/MM/yyyy HH:mm") + " v " + AwayTeam;
            return string.Empty;
        }
    }
}