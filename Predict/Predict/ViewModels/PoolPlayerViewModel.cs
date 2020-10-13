using Predict.Models;
using System.Collections.Generic;

namespace Predict.ViewModels
{
    public class PoolPlayerViewModel
    {
        public Pool Pool { get; set; }
        public List<PoolPlayer> PoolPlayers { get; set; }

        public int GlobalPoolId { get; set; }
    }
}