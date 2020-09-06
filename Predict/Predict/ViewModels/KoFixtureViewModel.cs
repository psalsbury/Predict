using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Predict.Helper;
using Predict.Models;

namespace Predict.ViewModels
{
    public class KoFixtureViewModel
    {
        public List<EventTeam> EventTeams { get; set; }
        public IList<string> Leagues { get; set; }
        public IList<short> RoundOfs { get; set; }

        public int Id { get; set; }

        public short EventId { get; set; }

        [Display(Name = "KO Fixture Date & Time")]
        [Required]
        public DateTime? FixtureDateTime { get; set; }

        [Display(Name = "Round Of")]
        [Required]
        public short RoundOf { get; set; } // How many teams are in this round //

        [Display(Name = "Position in the round")]
        [Required]
        public short Position { get; set; }

        [StringLength(50)]
        [Display(Name = "Team 1 From League")]
        public string Team1FromLeague { get; set; }

        [Display(Name = "Team 1 From League - Position")]
        public int? Team1FromLeaguePosition { get; set; }

        [Display(Name = "Team 1 - Fixture Id")]
        public int? Team1FromKoFixtureId { get; set; }

        [StringLength(50)]
        [Display(Name = "Team 2 From League")]
        public string Team2FromLeague { get; set; }

        [Display(Name = "Team 2 From League - Position")]
        public int? Team2FromLeaguePosition { get; set; }

        [Display(Name = "Team 2- Fixture Id")] public int? Team2FromKoFixtureId { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }
    }
}