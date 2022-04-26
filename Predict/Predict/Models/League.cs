using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class League
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [Required]
        [StringLength(25)]
        public string LeagueName { get; set; }

        [Required]
        [StringLength(15)]
        public string ShortLeagueName { get; set; }

        public int? RapidApiLeagueId { get; set; }

        public bool DailyRapidApiCheck { get; set; }

        public int? RapidApiV3LeagueSeasonId { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}