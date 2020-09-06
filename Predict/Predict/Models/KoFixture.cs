using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class KoFixture
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public short EventId { get; set; }

        [Column(TypeName = "DateTime2")] public DateTime FixtureDateTime { get; set; }

        public short RoundOf { get; set; } // How many teams are in this round //
        public short Position { get; set; }


        [StringLength(50)]
        public string Team1FromLeagueId { get; set; }  // comma delimetered list //
        public int? Team1FromLeaguePosition { get; set; }
        public int? Team1FromKoFixtureId { get; set; }

        [StringLength(50)]
        public string Team2FromLeagueId { get; set; } // comma delimetered list //
        public int? Team2FromLeaguePosition { get; set; }
        public int? Team2FromKoFixtureId { get; set; }

        public int? Team1Id { get; set; } // This will hold the actual result //
        public int? Team2Id { get; set; } // This will hold the actual result //

        [ForeignKey("Team1Id")]
        public Team Team1 { get; set; }

        [ForeignKey("Team2Id")]
        public Team Team2 { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}