using System;

namespace Predict.ViewModels
{
    public class PoolInfoViewModel
    {
        public int PoolId { get; set; }

        public short EventId { get; set; }

        public short TotalScore { get; set; }

        public string PoolName { get; set; }

        public short PoolPosition { get; set; }

        public string MemberInfo { get; set; }

        public DateTime? AdminApprovedDateTime { get; set; }

        public bool IsAdmin { get; set; }

        public int PlayersToApprove { get; set; }

        public int TotalNumberOfPlayers { get; set; }

        public bool FreezePredictions { get; set; }
    }
}