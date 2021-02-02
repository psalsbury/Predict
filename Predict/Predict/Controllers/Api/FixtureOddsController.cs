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
