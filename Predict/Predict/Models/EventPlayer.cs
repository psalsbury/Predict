using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    // Limits the teams that are available to an event
    public class EventPlayer
    {
        [Key] [Column(Order = 0)] public short EventId { get; set; }

        [ForeignKey("EventId")] public Event Event { get; set; }

        [Key] [Column(Order = 1)] public string PlayerId { get; set; }

        [Column(TypeName = "bit")]
        public Boolean Enabled { get; set; }

        //public int? FixturePredictionsEntered { get; set; }
        //public int? KoPredictionsEntered { get; set; }
        //public int? BonusPredictionsEntered { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}