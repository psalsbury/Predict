using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class EventPoolPlayersViewModel
    {

        public List<PoolPlayer> PoolPlayers { get; set; }
        public List<EventPoolPlayer> EventPoolPlayers { get; set; }
        public Pool Pool { get; set; }
        public Event Event { get; set; }
    }
}