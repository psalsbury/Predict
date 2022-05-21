using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class EventGeneration
    {
        [Key, Column(Order = 0)]
        public short EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [Key, Column(Order = 1)]
        public short LeagueEventGenerationId { get; set; }

        [ForeignKey("LeagueEventGenerationId")]
        public LeagueEventGeneration LeagueEventGeneration { get; set; }

        [Key, Column(Order = 2, TypeName = "datetime2")]
        public DateTime BaseStartDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime BaseEndDate { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}