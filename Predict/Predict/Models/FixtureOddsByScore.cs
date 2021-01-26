using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class FixtureOddsByScore
    {
        [Required]
        [Key]
        [Column(Order = 0)]
        public int RapidApiFixtureId { get; set; }

        [Required]
        [Key]
        [Column(Order = 1)]
        public short HomeScore { get; set; }

        [Required]
        [Key]
        [Column(Order = 2)]
        public short AwayScore { get; set; }

        [Required]
        public Decimal Odds { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}