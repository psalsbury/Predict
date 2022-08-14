using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.ViewModels
{
    public class PoolInfoFromSpViewModel
    {
        public string PoolName { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int CorrectScorePoints { get; set; }

        public int CorrectResultPoints { get; set; }

        public int WinMarginPoints { get; set; }

        public string JoinCode { get; set; }

        public string AdminDisplayName { get; set; }

        public int NbrPlayers { get; set; }

        public string MostRecentEventWinner { get; set; }

        public string MostRecentEventName { get; set; }

        public int NbrCompsEntered { get; set; }

	}
}