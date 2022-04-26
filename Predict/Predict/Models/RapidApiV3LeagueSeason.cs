using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class RapidApiV3LeagueSeason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [Required]
        public int RapidApiV3LeagueId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool Current { get; set; }

        [ForeignKey("RapidApiV3LeagueId")]
        public RapidApiV3League RapidApiV3League { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}