using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Predict.Models;


namespace Predict.Controllers.Api
{

    public class FixtureOddsByResultsController : ApiController
    {
        private readonly ApplicationDbContext _context;
        public FixtureOddsByResultsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("api/FixtureOddsByResults/GetOddsByResultsForFixture/{rapidApiFixtureId}")]
        public string GetOddsByResultsForFixture(int rapidApiFixtureId)
        {
            var fixtureOddsByResult= _context.FixtureOddsByResults.FirstOrDefault(o => o.RapidApiFixtureId == rapidApiFixtureId);

            var row = "<table class='table table-striped'><tr><td>{0}</td><td>" + fixtureOddsByResult.HomeOdds + "</td></tr>";
            row += "<tr><td>Draw</td><td>" + fixtureOddsByResult.DrawOdds + "</td></tr>";
            row += "<tr><td>{1}</td><td>" + fixtureOddsByResult.AwayOdds + "</td></tr><table>";

            // return html needed to show
            return row;
        }
    }
}
