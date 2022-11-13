using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class PoolHomeViewModel
    {
        public Pool Pool { get; set; }
        public List<EventPool> ActiveEventPools { get; set; }
        public List<EventPool> RecentEventPools { get; set; }
        public List<EventPoolPlayer> EventPoolPlayers { get; set; }
        public PoolPlayer PoolPlayer { get; set; }
        public List<PoolChat> PoolChats { get; set; }
    }
}