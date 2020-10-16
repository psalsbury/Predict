using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class EventPoolsViewModel
    {
        public List<EventPool> EventPools { get; set; }
        public List<Event> Events { get; set; }
        public int PoolId { get; set; }
    }
}