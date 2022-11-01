using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class EmailLead
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Player Name")]
        public string PlayerName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        public string WasAdmin { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EmailDate { get; set; }
    }
}