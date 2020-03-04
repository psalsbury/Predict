using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class FixturePredictionsViewModel
    {
        public List<FixturePrediction> FixturePredictions { get; set; }

        public List<EventTeam> EventTeams { get; set; }

        public string UserId { get; set; }

        public bool ReadOnly { get; set; }
    }

}