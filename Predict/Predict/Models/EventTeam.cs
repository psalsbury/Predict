using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    // Limits the teams that are available to an event
    public class EventTeam
    {
        [Key] [Column(Order = 0)] public int EventId { get; set; }

        [Key] [Column(Order = 1)] public int TeamId { get; set; }

        [ForeignKey("TeamId")] public Team Team { get; set; }

        [StringLength(50)] public string League { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}