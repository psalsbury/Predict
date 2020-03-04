using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
 public class StatsFixtureViewModel
    {
        public Fixture Fixture { get; set; }
        public List<StatFixturePrediction> StatFixturePredictions { get; set; }
    }

    public class StatFixturePrediction
    {
        public short HomePrediction { get; set; }
        public short AwayPrediction { get; set; }
        public int NumberOfPredictions { get; set; }
    }


}