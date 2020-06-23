using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class Player
    {
        [StringLength(128)] public string Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Your Name")]
        public string PlayerName { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [DefaultValue("false")] public bool PremiumPlayer { get; set; }

        [ForeignKey("Id")] public ApplicationUser AspNetUser { get; set; }

        [Column(TypeName = "datetime2")] public DateTime CreatedDateTime { get; set; }

        [Column(TypeName = "datetime2")] public DateTime ModifiedDateTime { get; set; }

        [Column(TypeName = "datetime2")] public DateTime? EmailConfirmedDateTime { get; set; }
    }
}