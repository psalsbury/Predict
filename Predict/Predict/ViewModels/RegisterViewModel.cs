using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class RegisterViewModel: Predict.Models.RegisterViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Player Name")]
        public string PlayerName { get; set; }

        public string Id { get; set; }

        [Display(Name = "Event")]
        public short EventId { get; set; }

        public IEnumerable<Team> Teams { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}