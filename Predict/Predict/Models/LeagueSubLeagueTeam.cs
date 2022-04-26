using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class LeagueSubLeagueTeam
    {

        [Key, Column(Order = 0)]
        [ForeignKey("LeagueSubLeague")]
        public short LeagueSubLeagueId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Team")]
        public int TeamId { get; set; }

        public Team Team { get; set; }

        [ForeignKey("LeagueSubLeagueId")]
        public LeagueSubLeague LeagueSubLeague { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}