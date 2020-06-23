using System.Collections.Generic;
using Predict.Models;

namespace Predict.ViewModels
{
    public class PoolPlayerViewModel
    {
        public Pool Pool { get; set; }
        public List<PoolPlayer> PoolPlayers { get; set; }
    }
}