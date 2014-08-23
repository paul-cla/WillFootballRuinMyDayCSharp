using System.Collections.Generic;

namespace WillFootballRuinMyDay
{
    public class FixtureHelpers
    {
        public List<Fixture> LimitToHomeTeam(IEnumerable<Fixture> fixtures, string teamName)
        {
            var newFixtures = new List<Fixture>();
            var i = 0;
            foreach (var fixture in fixtures)
            {
                if (fixture.HomeTeam == teamName)
                {
                    newFixtures.Add(fixture);
                    i++;
                }
            }
            return newFixtures;
        }
    }
}