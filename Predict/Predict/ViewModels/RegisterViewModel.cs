using Predict.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace Predict.ViewModels
{
    public class RegisterViewModel : Models.RegisterViewModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "The {0} cannot exceed {1} characters.", MinimumLength = 1)]
        [Display(Name = "Display Name")]

        public string DisplayName { get; set; }

        [Display(Name = "Player Name")] public string PlayerName { get; set; }

        public string JoinCode { get; set; }

        [Display(Name = "Comp")] public short EventId { get; set; }

        [Display(Name = "League")] public int PoolId { get; set; }

        public short defaultEventId { get; set; }

        public int defaultPoolId { get; set; }

        public IEnumerable<Team> Teams { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Pool> Pools { get; set; }
    }

    public class UpdateRegisterViewModel 
    {

        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} cannot exceed {1} characters.", MinimumLength = 1)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Supported Team")]
        public int? SupportTeamId { get; set; }

        public  List<Team> Teams { get; set; }
    }



    }