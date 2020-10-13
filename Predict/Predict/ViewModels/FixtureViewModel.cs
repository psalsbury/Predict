using Predict.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.ViewModels
{
    public class FixtureViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fixture Date/Time")]
        public DateTime? FixtureDateTime { get; set; }

        [Required(ErrorMessage = "Please select team")]
        [Display(Name = "Home Team")]
        public int HomeTeamId { get; set; }

        [Required(ErrorMessage = "Please select team")]
        [Display(Name = "Away Team")]
        public int AwayTeamId { get; set; }

        public Team HomeTeam { get; set; }

        [ForeignKey("AwayTeamId")] public Team AwayTeam { get; set; }

        [Display(Name = "Home Result")] public short? HomeResult { get; set; }

        [Display(Name = "Away Team")] public short? AwayResult { get; set; }

        public short LeagueId { get; set; }

        public League League { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }

        public List<Team> Teams { get; set; }
        public List<League> Leagues { get; set; }
    }
}