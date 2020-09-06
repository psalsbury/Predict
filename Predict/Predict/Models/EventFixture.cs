using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class EventFixture
    {
        [Key] [Column(Order = 0)]
        [Required] public short EventId { get; set; }

        [Key] [Column(Order = 1)]
        public int FixtureId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }
        
        [ForeignKey("FixtureId")]
        public Fixture Fixture { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}