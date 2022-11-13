using System;
using Predict.Models;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Predict.ViewModels
{
    public class FixturePredictionsViewModel
    {
        public List<FixturePrediction> FixturePredictions { get; set; }

        public List<int> RapidApiFixtureIdsWithResultOdds { get; set; }

        public List<int> RapidApiFixtureIdsWithScoreOdds { get; set; }

        public string UserId { get; set; }

        public bool OtherUserViewing { get; set; }

        public Event MyEvent { get; set; }

        public bool IsPremiumPlayer { get; set; }

        public bool AnyFixturesInTheFuture { get; set; }

        public Pool Pool { get; set; }

        public bool FreezeAllPredictions { get; set; }
    }
}