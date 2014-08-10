using System;

namespace WillFootballRuinMyDay
{
    internal class Fixture
    {
        public DateTime Date { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public override string ToString()
        {
            if (HomeTeam != null) return Date.ToString("dd/MM/yyyy") + " " + HomeTeam + " v " + AwayTeam;
            return string.Empty;
        }
    }
}