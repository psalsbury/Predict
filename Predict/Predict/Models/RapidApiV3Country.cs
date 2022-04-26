using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class RapidApiV3Country
    {

        [Key]
        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }

        [StringLength(10)]
        public string CountryCode{ get; set; }

        [StringLength(500)]
        public string Flag { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}