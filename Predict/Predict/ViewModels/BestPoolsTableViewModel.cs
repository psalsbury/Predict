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
        public decimal CorrectScore { get; set; }
        public decimal CorrectResult { get; set; }
        public decimal WinMargin { get; set; }
        public decimal KoScore { get; set; }
        public decimal TotalScore { get; set; }
    }
}