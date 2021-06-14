using System;
using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class BonusQuestionPredictionsViewModel
    {
        public List<BonusQuestionPrediction> BonusQuestionPredictions { get; set; }
        public string PlayerId { get; set; }
        public short EventId { get; set; }
        public bool ReadOnly { get; set; }
        public bool IsPremiumPlayer { get; set; }
        public DateTime EventStartDateTime { get; set; }

        public DateTime EventEndDateTime { get; set; }
        public bool FreezeAllPredictions { get; set; }
    }
}