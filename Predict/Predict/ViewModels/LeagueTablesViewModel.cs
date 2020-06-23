using System.Collections.Generic;
using Predict.Helper;

namespace Predict.ViewModels
{
    public class LeagueTablesViewModel
    {
        public List<LeagueTable> LeagueTables { get; set; }
        public bool Results { get; set; }
        public bool IsPremiumPlayer { get; set; }
    }
}