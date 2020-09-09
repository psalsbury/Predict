namespace Predict.ViewModels
{
    public class TableViewModel
    {
        public long PoolPosition { get; set; }
        public string PlayerId { get; set; }
        public string DisplayName { get; set; }
        public int PoolId { get; set; }
        public short EventId { get; set; }
        public string PoolName { get; set; }
        public int? SupportTeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public int CorrectScore { get; set; }
        public int CorrectResult { get; set; }
        public int WinMargin { get; set; }
        public int KoScore { get; set; }
        public int TotalScore { get; set; }
    }

}