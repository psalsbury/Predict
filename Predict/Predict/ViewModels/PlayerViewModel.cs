using Predict.Models;

namespace Predict.ViewModels
{
    public class PlayerViewModel
    {
        public Player Player { get; set; }
        public ApplicationUser AspNetUser { get; set; }
    }
}