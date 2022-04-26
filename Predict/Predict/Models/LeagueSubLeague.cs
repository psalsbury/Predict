using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class LeagueSubLeague
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [Required]
        public short LeagueId { get; set; }

        [ForeignKey("LeagueId")] 
        public League Leauge { get; set; }

        [Required]
        [StringLength(25)]
        public string SubLeagueName { get; set; }

        [Required]
        [StringLength(15)]
        public string SubLeagueShortName { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}