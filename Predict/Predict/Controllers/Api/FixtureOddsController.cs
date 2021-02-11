using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Predict.Dtos;
using Predict.Helper;
using Predict.Models;
using Quartz.Impl.Matchers;
using System.Data.Entity;

namespace Predict.Controllers.Api
{

    public class FixtureOddsController : ApiController
    {
        private readonly ApplicationDbContext _context;
        public FixtureOddsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("api/FixtureOdds/GetBettingResultsForComp/{eventId}/{playerId}")]

        public string GetBettingResultsForComp(int eventId, string playerId)
        {
            if (!User.Identity.IsAuthenticated)
                throw new Exception("User not authorized");

            Decimal moneyWonResults=0;
            Decimal moneyWonScore = 0;
            short correctResults = 0;
            short correctScores = 0;

            // count of number of fixtures in this event with results
            var eventFixtures = _context.EventFixtures
                .Include(f => f.Fixture)
                .Where(a => a.EventId == eventId)
                .Where(f => f.Fixture.ResultProcessed == true);

            var fixturePredictions = (from m in _context.FixturePredictions where m.PlayerId==playerId && eventFixtures.Any(a => a.FixtureId==m.FixtureId) select m).ToList();

            var nbrFixturesPredicted = fixturePredictions.Count;

            var listOfIds = (from m in eventFixtures where m.EventId==eventId && m.Fixture.ResultProcessed==true && m.Fixture.RapidApiFixtureId != null select m.Fixture.RapidApiFixtureId);
            var listOfRapidApiFixturesWithResultOdds = (from m in _context.FixtureOddsByResults where listOfIds.Contains(m.RapidApiFixtureId) select m).Distinct().ToList();
            var listOfRapidApiFixturesWithScoreOdds = (from m in _context.FixtureOddsByScores where listOfIds.Contains(m.RapidApiFixtureId) select m).Distinct().ToList();

            foreach (var eventFixture in eventFixtures)
            {

                var fixturePrediction =
                    fixturePredictions.FirstOrDefault(f => f.FixtureId == eventFixture.FixtureId);

                if (fixturePrediction != null)
                {

                    // Check if the actual score was correct
                    if (fixturePrediction.HomePrediction == eventFixture.Fixture.HomeResult &&
                        fixturePrediction.AwayPrediction == eventFixture.Fixture.AwayResult)
                    {

                        var scoreOdds = listOfRapidApiFixturesWithScoreOdds
                            .FirstOrDefault(a => a.RapidApiFixtureId == eventFixture.Fixture.RapidApiFixtureId
                                                 && a.HomeScore == fixturePrediction.HomePrediction
                                                 && a.AwayScore == fixturePrediction.AwayPrediction);

                        if (scoreOdds != null)
                        {
                            correctScores += 1;
                            moneyWonScore += scoreOdds.Odds;
                        }
                    }

                    // Now check if the result was correct
                    var predictedResult = "draw";
                    var actualResult = "draw";
                    
                    if (fixturePrediction.HomePrediction > fixturePrediction.AwayPrediction)
                    {
                        predictedResult = "home";
                    }
                    else if (fixturePrediction.HomePrediction < fixturePrediction.AwayPrediction)
                    {
                        predictedResult = "away";
                    }

                    if (eventFixture.Fixture.HomeResult > eventFixture.Fixture.AwayResult)
                    {
                        actualResult = "home";
                    }
                    else if (eventFixture.Fixture.HomeResult < eventFixture.Fixture.AwayResult)
                    {
                        actualResult = "away";
                    }

                    var resultOdds = listOfRapidApiFixturesWithResultOdds
                        .FirstOrDefault(a => a.RapidApiFixtureId == eventFixture.Fixture.RapidApiFixtureId);

                    if (resultOdds != null)
                    {
                        if (predictedResult == actualResult)
                        {
                            correctResults += 1;
                            if (predictedResult == "home")
                            {
                                moneyWonResults += resultOdds.HomeOdds;
                            }
                            else if (predictedResult == "draw")
                            {
                                moneyWonResults += resultOdds.DrawOdds;
                            }
                            else if (predictedResult == "away")
                            {
                                moneyWonResults += resultOdds.AwayOdds;
                            }
                        }
                    }
                }

            }

            var profitLossResults = (moneyWonResults - Convert.ToDecimal(nbrFixturesPredicted));
            var profitLossScores = (moneyWonScore - Convert.ToDecimal(nbrFixturesPredicted));
            var profitLossResultsFont = "";
            var profitLossScoresFont = "";

            profitLossResultsFont = "<font style='font-size: 20px; color:" + (profitLossResults > 0 ? "green": (profitLossResults==0 ? "black" : "red")) + ";'>";

            profitLossScoresFont = "<font style='font-size: 20px; color:" + (profitLossScores > 0 ? "green" : (profitLossScores == 0 ? "black" : "red")) + ";'>";

            var row = "<h3>If you had bet £1 on each fixture...</h3>"
                      + "<table class='table table-bordered'>"
                      + "<tr><td>&nbsp;</td><td><b>Result Bets</b></td><td><b>Correct Score Bets</b></td></tr>"
                      + "<tr><td>Nbr Fixtures</td><td>" + nbrFixturesPredicted.ToString() + "</td><td>"
                      + nbrFixturesPredicted.ToString() + "</td></tr>"
                      + "<tr><td>Nbr Correct</td><td>" + correctResults + "</td><td>" + correctScores + "</td></tr>"
                      + "<tr><td>Bet Amount</td><td>£" + nbrFixturesPredicted.ToString() + "</td><td>£"
                      + nbrFixturesPredicted.ToString() + "</td></tr>"
                      + "<tr><td>Return</td><td>£" + moneyWonResults + "</td><td>£" + moneyWonScore + "</td></tr>"
                      + "<tr><td>Profit/Loss</td><td>" + profitLossResultsFont + "£" + profitLossResults + (profitLossResultsFont != ""?"</font>":"")
                      + "</td><td>" + profitLossScoresFont + "£" + profitLossScores +(profitLossScoresFont != "" ? "</font>" : "") + "</td></tr>"
                      + "</table>";

            return row;

        }

        [HttpGet]
        [Route("api/FixtureOdds/GetOddsByResultsForFixture/{rapidApiFixtureId}/{homeResult}/{awayResult}/{homePrediction}/{awayPrediction}/{otherUserViewing}")]
        public string GetOddsByResultsForFixture(int rapidApiFixtureId, int homeResult, int awayResult, int homePrediction, int awayPrediction, bool otherUserViewing)
        {
            if (!User.Identity.IsAuthenticated)
                throw new Exception("User not authorized");

            var green = "#E8FBE1";
            var red = "#FFDBDB";
            var amber = "#FAF8DF";

            var homeColor = "";
            var drawColor = "";
            var awayColor = "";
            var predCorrect = false;
            string row = "";

            var fixtureOddsByResult =
                _context.FixtureOddsByResults.FirstOrDefault(o => o.RapidApiFixtureId == rapidApiFixtureId);

            // Only calculate the colour of the row if its the user looking at his own predictions
            if (!otherUserViewing)
            {
                if (homeResult >= 0)
                {
                    if (homeResult > awayResult)
                    {
                        homeColor = green;
                    }
                    else if (homeResult == awayResult)
                    {
                        drawColor = green;
                    }
                    else
                    {
                        awayColor = green;
                    }
                }

                if (homePrediction >= 0)
                {
                    if (homePrediction > awayPrediction)
                    {
                        if (homeColor != green && homeResult >= 0)
                        {
                            homeColor = red;
                        }
                        else if (homeResult == -1)
                        {
                            homeColor = amber;
                        }
                    }
                    else if (homePrediction == awayPrediction)
                    {

                        if (drawColor != green && homeResult >= 0)
                        {
                            drawColor = red;
                        }
                        else if (homeResult == -1)
                        {
                            drawColor = amber;
                        }
                    }
                    else
                    {

                        if (awayColor != green && homeResult >= 0)
                        {
                            awayColor = red;
                        }
                        else if (homeResult == -1)
                        {
                            awayColor = amber;
                        }
                    }
                }
            }

            var fixtureOddsByScore = _context.FixtureOddsByScores.Where(a => a.RapidApiFixtureId == rapidApiFixtureId
                                                                             && ((a.HomeScore == homeResult &&
                                                                                     a.AwayScore == awayResult) ||
                                                                                 (a.HomeScore == homePrediction &&
                                                                                     a.AwayScore == awayPrediction)));

            if (fixtureOddsByResult != null)
            {

                row = "<table class='table table-bordered'><tr bgcolor='" + homeColor + "'><td>{0}</td><td>" + fixtureOddsByResult.HomeOdds +
                          "</td></tr>";
                row += "<tr bgcolor='" + drawColor + "'><td>Draw</td><td>" + fixtureOddsByResult.DrawOdds + "</td></tr>";
                row += "<tr bgcolor='" + awayColor + "'><td>{1}</td><td>" + fixtureOddsByResult.AwayOdds + "</td></tr></table>";
            }

            // If another user is viewing the odds for a player, then dont show the odds for the predicted score;
            if (otherUserViewing)
                return row;

            var predictionScore = fixtureOddsByScore.FirstOrDefault(a => a.HomeScore == homePrediction && a.AwayScore == awayPrediction);
            if (predictionScore != null)
            {
                if(homeResult==homePrediction && awayResult==awayPrediction)
                {
                    row += "<p><i>Predicted & Actual scoreline odds</i><p>";
                    predCorrect = true;
                }
                else
                {
                    row += "<p><i>Predicted scoreline odds</i><p>";
                }

                row += "<table class='table table-bordered'><tr bgcolor='"+ (predCorrect?green: (homeResult>=0?red:amber))+"'><td width='25%'>{0}</td><td width='15%'>" + predictionScore.HomeScore +
                       "<td  width='25%'>{1}</td><td width='15%'>" + predictionScore.AwayScore +
                       "<td width='20%'>" + predictionScore.Odds + "</td></tr></table>";
            }

            if (homeResult != homePrediction || awayResult != awayPrediction)
            {
                var resultScore = fixtureOddsByScore.FirstOrDefault(a => a.HomeScore == homeResult && a.AwayScore == awayResult);
                if (resultScore != null)
                {
                    row += "<p><i>Actual scoreline odds</i><p> " +
                            "<table class='table table-bordered'><tr bgcolor='"+green+"'><td width='25%'>{0}</td><td width='15%'>" + resultScore.HomeScore +
                           "<td width='25%'>{1}</td><td width='15%'>" + resultScore.AwayScore +
                           "<td width='20%'>" + resultScore.Odds + "</td></tr></table>";
                }
            }

            // return html needed to show
            return row;
        }
    }
}
