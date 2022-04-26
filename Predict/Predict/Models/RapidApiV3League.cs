using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class RapidApiV3League
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [StringLength(500)]
        public string Logo { get; set; }

        [Required]
        [StringLength(500)]
        public string CountryName { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CountryName")]
        public RapidApiV3Country RapidApiV3Country { get; set; }
    }
}