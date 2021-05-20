using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.ViewModels
{
    public class BestPoolsTableViewModel
    {
        public long PoolPosition { get; set; }
        public  int NbrOfMembers { get; set; }
        public string PoolName { get; set; }
        public int PoolId { get; set; }
        public short EventId { get; set; }
        public int CorrectScore { get; set; }
        public int CorrectResult { get; set; }
        public int WinMargin { get; set; }
        public int KoScore { get; set; }
        public int TotalScore { get; set; }
    }
}