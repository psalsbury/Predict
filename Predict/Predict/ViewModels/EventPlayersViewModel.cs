using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.ViewModels
{
    public class EventPlayersViewModel
    {
        public short EventId { get; set; }

		public string EventName { get; set; }

		public string EventDescription { get; set; }

		public DateTime StartDateTime { get; set; }

		public DateTime EndDateTime { get; set; }

		public string CreatedBy { get; set; }

		public int Fixtures { get; set; }

		public int NbrPlayers { get; set; }


	}
}
