using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class EventFixturesViewModel
    {
        public List<Fixture> Fixtures { get; set; }
        public List<EventFixture> EventFixtures { get; set; }
        public Event Event { get; set; }
    }
}