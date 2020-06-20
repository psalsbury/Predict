using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Predict.ViewModels
{
    public class PoolInfoViewModel
    {
        public int PoolId { get; set; }

        public short EventId { get; set; }

        public Int16 TotalScore { get; set; }

        public string PoolName { get; set; }

        public Int16 PoolPosition { get; set; }

        public string MemberInfo { get; set; }

        public DateTime? AdminApprovedDateTime { get; set; }

        public bool IsAdmin { get; set; }

        public int PlayersToApprove { get; set; }

        public int TotalNumberOfPlayers { get; set; }

    }
}