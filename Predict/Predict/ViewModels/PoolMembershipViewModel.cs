using System.Collections.Generic;
using Predict.Models;

namespace Predict.ViewModels
{
    public class PoolMembershipViewModel
    {
        public PoolMembershipViewModel()
        {
            ReadOnlyPools = new List<int>();
        }

        public List<Pool> Pools { get; set; }
        public List<PoolPlayer> JoinedPools { get; set; }
        public string PlayerId { get; set; }
        public List<int> ReadOnlyPools { get; set; }
    }
}