using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class TableViewModel
    {
        public Int64 PoolPosition { get; set; }
        public string PlayerId { get; set; }
        public string DisplayName { get; set; }
        public int? PoolId { get; set; }
        public string PoolName { get; set; }
        public int? SupportTeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public int CorrectScore { get; set; }
        public int CorrectResult { get; set; }
        public int KoScore { get; set; }
        public int TotalScore { get; set; }

    }

    public class BestPoolsTableViewModel
    {
        public Int64 PoolPosition { get; set; }
        public int PoolId { get; set; }
        public string PoolName { get; set; }
        public int CorrectScore { get; set; }
        public int CorrectResult { get; set; }
        public int KoScore { get; set; }
        public int TotalScore { get; set; }
        public int NbrPlayers { get; set; }
    }


    public class BestTeamsTableViewModel
    {
        public Int64 PoolPosition { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public int CorrectScore { get; set; }
        public int CorrectResult { get; set; }
        public int KoScore { get; set; }
        public int TotalScore { get; set; }
        public int NbrPlayers { get; set; }

    }

}