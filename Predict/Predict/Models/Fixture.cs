using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    // A fixture is available to be linked to any number of events
    public class Fixture
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        [Display(Name = "Fixture Date & Time")]
        public DateTime FixtureDateTime { get; set; }

        [Display(Name = "Home Team")] public int? HomeTeamId { get; set; }

        [Display(Name = "Away Team")] public int? AwayTeamId { get; set; }

        [ForeignKey("HomeTeamId")] public Team HomeTeam { get; set; }

        [ForeignKey("AwayTeamId")] public Team AwayTeam { get; set; }

        [Display(Name = "Home Result")] public short? HomeResult { get; set; }

        [Display(Name = "Away Result")] public short? AwayResult { get; set; }

        [Required]
        public short LeagueId { get; set; }

        [Required]
        public bool ResultProcessed { get; set; } // if the result has been processed and predictions updated with the score

        [ForeignKey("LeagueId")]
        public League League { get; set; }

        public int? RapidApiFixtureId { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        [StringLength(50)]
        public string RapidApiLongStatus { get; set; }

        public bool FixtureDatePassed => FixtureDateTime < DateTime.UtcNow; // FixtureDateTime should already be UTC
    }
}