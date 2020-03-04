using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Predict.Models
{
    // Player predictions for fixtures (that are linked to an event)
    public class FixturePrediction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [Required]
        public int FixtureId { get; set; }

        [ForeignKey("FixtureId")]
        public Fixture Fixture { get; set; }

        [Required]
        [StringLength(128)]
        public string PlayerId { get; set; }

        [Required]
        public short? HomePrediction { get; set; } // Set to nullable as the initial view will need to show blanks

        [Required]
        public short? AwayPrediction{ get; set; } // Set to nullable as the initial view will need to show blanks

        public bool? CorrectScore { get; set; } 
        public bool? CorrectResult { get; set; }
        public bool? CorrectWinMargin { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}