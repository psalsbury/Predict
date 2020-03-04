using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class Player
    {
        [StringLength(128)]
        public string Id { get; set; }

        [Required] [StringLength(100)]
        [Display(Name = "Your Name")]
        public string PlayerName { get; set; }

        [Required] [StringLength(30)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public int? SupportTeamId { get; set; }

        [ForeignKey("SupportTeamId")]
        [Display(Name = "Supported Team")]
        public Team SupportTeam { get; set; }

        [DefaultValue("false")]
        public bool PremiumPlayer { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser AspNetUser { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EmailConfirmedDateTime { get; set; }

    }
}