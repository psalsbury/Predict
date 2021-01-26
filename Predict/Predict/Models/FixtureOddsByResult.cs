using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class FixtureOddsByResult
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RapidApiFixtureId { get; set; }

        [Required]
        public Decimal HomeOdds { get; set; }

        [Required]
        public Decimal DrawOdds { get; set; }

        [Required]
        public Decimal AwayOdds { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}