using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class FixturePredictionsViewModel
    {
        public List<FixturePrediction> FixturePredictions { get; set; }

        public string UserId { get; set; }

        public bool OtherUserViewing { get; set; }

        public short EventId { get; set; }

        public bool IsPremiumPlayer { get; set; }

        public bool AnyFixturesInTheFuture { get; set; }

        public Pool Pool { get; set; }
    }
}