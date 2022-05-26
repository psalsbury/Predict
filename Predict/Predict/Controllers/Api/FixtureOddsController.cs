using System;
using System.Linq;
using System.Web.Http;
using Predict.Models;
using System.Data.Entity;
using System.Collections.Generic;

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

            var fixturePredictions = (from m in _context.FixturePredictions where m.PlayerId==playerId && m.EventId==eventId && eventFixtures.Any(a => a.FixtureId==m.FixtureId) select m).ToList();

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
            var blue = "#B4CFEC";

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

            var allFixtureOddsByScore = _context.FixtureOddsByScores.Where(a => a.RapidApiFixtureId == rapidApiFixtureId && a.Odds != 0).ToList();
            var fixtureOddsByScore = allFixtureOddsByScore.Where(a => ((a.HomeScore == homeResult &&
                                                                        a.AwayScore == awayResult) 
                                                                        ||
                                                                       (a.HomeScore == homePrediction &&
                                                                        a.AwayScore == awayPrediction)));
            row = "<div>"
                     + "<p class='alignleft' style='vertical-align: middle;'><i>Odds refreshed daily up until kick off</i><p>";

            if(allFixtureOddsByScore.Count>0)
            {
                row += "<span class='btn btn-primary alignright' id='petebutton'>&nbsp;<i class='fa fa-info'></i></span>";
            }

            row += "</div><br><br>";

            row += "<div id='playerinfo'>";
            if (fixtureOddsByResult != null)
            {

                row += "<table class='table table-bordered' style='font-size: 11px;'>"
                        + "<tr bgcolor='" + homeColor + "'>"
                            + "<td>{0}</td>"
                            + "<td>" + fixtureOddsByResult.HomeOdds + "</td>"
                        + "</tr>";
                row += "<tr bgcolor='" + drawColor + "'>"
                        + "<td>Draw</td>"
                        + "<td>" + fixtureOddsByResult.DrawOdds + "</td>"
                    + "</tr>";
                row += "<tr bgcolor='" + awayColor + "'>"
                        + "<td>{1}</td>"
                        + "<td>" + fixtureOddsByResult.AwayOdds + "</td>"
                        + "</tr>"
                    + "</table>";
            }

            // If another user is viewing the odds for a player, then dont show the odds for the predicted score;
            if (otherUserViewing)
                return row;

            var predictionScore = fixtureOddsByScore.FirstOrDefault(a => a.HomeScore == homePrediction && a.AwayScore == awayPrediction && a.Odds > 0);
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

                row += "<table class='table table-bordered' style='font-size: 11px;'>"
                        + "<tr bgcolor='"+ (predCorrect?green: (homeResult>=0?red:amber))+"'>"
                            + "<td width='32%'>{0}</td>"
                            + "<td width='8%'>" + predictionScore.HomeScore + "</td>"
                            + "<td width='32%'>{1}</td>"
                            + "<td width='8%'>" + predictionScore.AwayScore + "</td>"
                            + "<td width='20%'>" + predictionScore.Odds + "</td>"
                        + "</tr>"
                        + "</table>";
            }

            if (homeResult != homePrediction || awayResult != awayPrediction)
            {
                var resultScore = fixtureOddsByScore.FirstOrDefault(a => a.HomeScore == homeResult && a.AwayScore == awayResult && a.Odds > 0);
                if (resultScore != null)
                {
                    row += "<p><i>Actual scoreline odds</i><p> " +
                            "<table class='table table-bordered' style='font-size: 11px;'>"
                                + "<tr bgcolor='" + green +"'>"
                                    + "<td width='32%'>{0}</td>"
                                    + "<td width='8%'>" + resultScore.HomeScore + "</td>"
                                    + "<td width='32%'>{1}</td>"
                                    + "<td width='8%'>" + resultScore.AwayScore + "</td>"
                                    + "<td width='20%'>" + resultScore.Odds + "</td>"
                                + "</tr>"
                             + "</table>";
                }
            }
            row += "</div>";
            row += "<div id='allodds'>";



            for (int i = 1; i <= 3; i++)
            {
                List<FixtureOddsByScore> tablsAllFixtureOddsByScore;
                if (i==1)
                {
                    tablsAllFixtureOddsByScore = allFixtureOddsByScore.Where(a => a.HomeScore > a.AwayScore).OrderBy(a => a.Odds).ToList(); ;
                    row += "<b><i>{0} win</i></b>";
                }
                else if (i == 2)
                {
                    tablsAllFixtureOddsByScore = allFixtureOddsByScore.Where(a => a.HomeScore < a.AwayScore).OrderBy(a => a.Odds).ToList();
                    row += "<b><i>{1} win</i></b>";
                }
                else
                {
                    tablsAllFixtureOddsByScore = allFixtureOddsByScore.Where(a => a.HomeScore == a.AwayScore).OrderBy(a => a.Odds).ToList();
                    row += "<b><i>Game is a draw</i></b>";
                }

                row += "<table class='table' style='font-size: 11px;'>";
                foreach (var thisFixtureOddsByScore in tablsAllFixtureOddsByScore)
                {
                    if (thisFixtureOddsByScore.HomeScore == homeResult && thisFixtureOddsByScore.AwayScore == awayResult)
                    {
                        row += "<tr bgcolor='" + blue + "'>";
                    }
                    else
                    {
                        row += "<tr>";
                    }
                    row += "<td width='32%'>{0}</td>"
                            + "<td width='8%'>" + thisFixtureOddsByScore.HomeScore + "</td>"
                            + "<td width='32%'>{1}</td>"
                            + "<td width='8%'>" + thisFixtureOddsByScore.AwayScore + "</td>"
                            + "<td width='20%'>" + thisFixtureOddsByScore.Odds + "</td>"
                        + "</tr>";
                }
                row += "</table>";
            }


            row += "</div>";
            
            // return html needed to show
            return row;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
