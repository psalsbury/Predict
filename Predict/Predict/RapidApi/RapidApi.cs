using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using Predict.RapidAPIFixturesByLeague;
using Predict.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using Fixture = Predict.RapidAPIFixturesByLeague.Fixture;

namespace Predict.RapidApi
{
    public class RapidApi
    {

        private readonly ApplicationDbContext _context;

        public RapidApi()
        {
            _context = new ApplicationDbContext();
        }

        public void FixturesByLeague(int rapidApiLeagueId, string leagueName, string shortLeagueName)
        {
            rapidApiLeagueId = 2790;
            leagueName = "Premier League 2020/21";
            shortLeagueName = "Premier League";

            var client = new RestClient("https://api-football-v1.p.rapidapi.com/v2/fixtures/league/"+ rapidApiLeagueId + "?timezone=Europe%2FLondon");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d2f34fda45msh6212d84073d1f56p1b7dd9jsn619a512aa3fb");
            IRestResponse response = client.Execute(request);

            var pete = new JsonSerializer();
            var pete2 = pete.Deserialize<Root>(response);

            var fixtures = _context.Fixtures.ToList();
            var teams = _context.Teams.ToList();
            var newResultFound = false;

            var league = _context.Leagues.FirstOrDefault(f => f.RapidApiLeagueId == rapidApiLeagueId);
            if (league == null)
            {
                league = new Models.League();
                league.LeagueName = leagueName;
                league.ShortLeagueName = shortLeagueName;
                league.CreatedDateTime = DateTime.Now;
                league.ModifiedDateTime = DateTime.Now;
                _context.Leagues.Add(league);
                _context.SaveChanges();
            }

            foreach (Fixture rapidApiFixture in pete2.api.fixtures)
            {
                var rapidApiFixtureId = rapidApiFixture.fixture_id;
                var updateDb = false;
                var homeTeam = CheckTeamExists(teams, rapidApiFixture.homeTeam.team_id,
                    rapidApiFixture.homeTeam.team_name, rapidApiFixture.homeTeam.logo);

                if (homeTeam.Id == 0)
                {
                    _context.Teams.AddOrUpdate(homeTeam);
                    updateDb = true;
                }

                var awayTeam = CheckTeamExists(teams, rapidApiFixture.awayTeam.team_id,
                    rapidApiFixture.awayTeam.team_name, rapidApiFixture.awayTeam.logo);

                if (awayTeam.Id == 0)
                {
                    _context.Teams.AddOrUpdate(awayTeam);
                    updateDb = true;
                }

                if(updateDb)
                    _context.SaveChanges();

                var fixture = fixtures.FirstOrDefault(f => f.RapidApiFixtureId == rapidApiFixtureId);
                if (fixture == null)
                {
                    fixture = new Models.Fixture
                    {
                        RapidApiFixtureId = rapidApiFixtureId,
                        HomeTeamId = homeTeam.Id,
                        AwayTeamId = awayTeam.Id,
                        CreatedDateTime = DateTime.Now,
                        ModifiedDateTime = DateTime.Now
                    };
                }

                if (fixture.HomeResult == null && rapidApiFixture.score.fulltime != null)
                {
                    newResultFound = true;
                }
                fixture.FixtureDateTime = rapidApiFixture.event_date;
                fixture.LeagueId = league.Id;
                fixture.HomeResult = (short)rapidApiFixture.goalsHomeTeam;
                fixture.AwayResult = (short)rapidApiFixture.goalsAwayTeam;
                fixture.ResultProcessed = false;

                _context.Fixtures.AddOrUpdate(fixture);

            }
            _context.SaveChanges();
        }

        private Models.Team CheckTeamExists(List<Models.Team> teams, int rapidApiTeamId, string teamName, string logo)
        {
            var team = _context.Teams.FirstOrDefault(t => t.RapidApiTeamId == rapidApiTeamId);
            if (team == null)
            {
                team = new Models.Team
                {
                    RapidApiTeamId = rapidApiTeamId,
                    TeamName = teamName,
                    TeamFlag = logo,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
                };
            }

            return team;
        }
  
    }
}