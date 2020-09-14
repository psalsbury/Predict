using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class EventFixturesViewModel
    {
        public List<Fixture> Fixtures { get; set; }
        public List<EventFixture> EventFixtures { get; set; }
        public Event Event { get; set; }
    }
}