using System;

namespace Predict.RapidApi
{
    public class RapidApiResultCheck
    {
        public int RapidApiLeagueId { get; set; }
        public DateTime FixtureDateTime { get; set; }
        public DateTime ResultCheckDateTime { get; set; }
    }
}