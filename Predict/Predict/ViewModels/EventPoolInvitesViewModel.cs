using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class EventPoolInvitesViewModel
    {
        public Pool Pool { get; set; }
        public List<PoolPlayer> PoolPlayers { get; set; }
        public List<EventPoolPlayer> EventPoolPlayers { get; set; }
        public List<EmailRequestToJoin> EmailRequestToJoins { get; set; }
        public int PoolId { get; set; }
        public short EventId { get; set; }
    }
}