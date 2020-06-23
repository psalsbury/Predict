using System.Collections.Generic;
using Predict.Models;

namespace Predict.ViewModels
{
    public class GroupGameResultsViewModel
    {
        public List<Fixture> Fixtures { get; set; }

        public List<EventTeam> EventTeams { get; set; }
    }
}