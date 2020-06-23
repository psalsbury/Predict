namespace Predict.Helper
{
    public class LeagueTableTeam
    {
        public int TeamId { get; set; }
        public string Team { get; set; }
        public short Played { get; set; }
        public short Won { get; set; }
        public short Draws { get; set; }
        public short Lost { get; set; }
        public short GoalsFor { get; set; }
        public short GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;

        public string TeamFlag { get; set; }
        public short Points { get; set; }
        public short Position { get; set; }
    }
}