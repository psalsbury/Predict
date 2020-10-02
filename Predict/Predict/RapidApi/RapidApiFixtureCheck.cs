using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.RapidApi
{
    // Used to control the daily update of all fixtures for each league
    public class RapidApiFixtureCheck
    {
        public int RapidApiLeagueId { get; set; }
        public DateTime DateLastChecked { get; set; }
    }
}