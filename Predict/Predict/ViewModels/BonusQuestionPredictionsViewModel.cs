using System.Collections.Generic;
using Predict.Models;

namespace Predict.ViewModels
{
    public class BonusQuestionPredictionsViewModel
    {
        public List<BonusQuestionPrediction> BonusQuestionPredictions { get; set; }
        public string PlayerId { get; set; }
        public short EventId { get; set; }
        public bool ReadOnly { get; set; }
        public bool IsPremiumPlayer { get; set; }
    }
}