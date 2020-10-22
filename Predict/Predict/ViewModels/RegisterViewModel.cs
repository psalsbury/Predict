using Predict.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Predict.ViewModels
{
    public class RegisterViewModel : Models.RegisterViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Player Name")] public string PlayerName { get; set; }

        public string Id { get; set; }

        [Display(Name = "Comp")] public short EventId { get; set; }

        public IEnumerable<Team> Teams { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}