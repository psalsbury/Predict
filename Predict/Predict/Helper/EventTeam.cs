using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.Helper
{
    public class EventTeam
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public short LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string ShortLeagueName { get; set; }
        public string FlagFileLocation { get; set; }
    }
    
}