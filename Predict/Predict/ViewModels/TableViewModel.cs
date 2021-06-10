using System.Security.AccessControl;

namespace Predict.ViewModels
{
    public class TableViewModel
    {
        public long PoolPosition { get; set; }
        public string PlayerId { get; set; }
        public string DisplayName { get; set; }
        public int? SupportTeamId { get; set; }
        public string SupportTeamName { get; set; }
        public string SupportTeamFlag { get; set; }
        public int PoolId { get; set; }
        public short EventId { get; set; }
        public int CorrectScore { get; set; }
        public int CorrectResult { get; set; }
        public int WinMargin { get; set; }
        public int KoScore { get; set; }
        public int TotalScore { get; set; }
        public int? FixturePredictionsEntered { get; set; }
        public int? KoPredictionsEntered { get; set; }
        public int? BonusPredictionsEntered { get; set; }
    }

}