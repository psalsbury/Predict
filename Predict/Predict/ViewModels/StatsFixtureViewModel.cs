using System.Collections.Generic;
using Predict.Models;

namespace Predict.ViewModels
{
    public class StatsFixtureViewModel
    {
        public Fixture Fixture { get; set; }
        public List<StatFixturePrediction> StatFixturePredictions { get; set; }
        public short EventId { get; set; }
    }

    public class StatFixturePrediction
    {
        public short HomePrediction { get; set; }
        public short AwayPrediction { get; set; }
        public int NumberOfPredictions { get; set; }
    }
}