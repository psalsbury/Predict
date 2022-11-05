using Predict.Helper;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class LeagueTablesViewModel
    {
        public List<LeagueTable> LeagueTables { get; set; }
        public bool Results { get; set; }
        public bool IsPremiumPlayer { get; set; }
        public bool OtherUserViewing { get; set; }
    }
}