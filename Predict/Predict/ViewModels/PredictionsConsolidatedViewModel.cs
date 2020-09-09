using Predict.Models;

namespace Predict.ViewModels
{
    public class PredictionsConsolidatedViewModel
    {
        public string PlayerId { get; set; }
        public short EventId { get; set; }
        public Player Player { get; set; }
        public Pool Pool { get; set; }
        public KoFixturePredictionViewModel KoFixturePredictionViewModel { get; set; }
        public FixturePredictionsViewModel FixturePredictionsViewModel { get; set; }
        public LeagueTablesViewModel LeagueTablesViewModel { get; set; }
        public BonusQuestionPredictionsViewModel BonusQuestionPredictionsViewModel { get; set; }
    }
}