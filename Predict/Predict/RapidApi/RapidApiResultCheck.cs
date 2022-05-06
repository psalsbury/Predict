using System;

namespace Predict.RapidApi
{
    public class RapidApiResultCheck
    {
        public int? RapidApiLeagueId { get; set; }
        public int? RapidApiV3LeagueSeasonId { get; set; }
        public int? RapidApiV3LeagueId { get; set; }
        public int? Year { get; set; }
        public short LeagueId { get; set; }
        public DateTime FixtureDateTime { get; set; }
        public DateTime ResultCheckDateTime { get; set; }
    }
}