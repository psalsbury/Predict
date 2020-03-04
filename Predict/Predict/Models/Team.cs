using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{

    public class Team
    {
        private string _flagFileLocation;
        private string _animatedFlagLocation;
        private string _homeFolder;

        public Team()
        {
            _flagFileLocation = _homeFolder + System.Configuration.ConfigurationManager.AppSettings["FlagFileLocation"];
            _animatedFlagLocation = _homeFolder + System.Configuration.ConfigurationManager.AppSettings["AnimatedFlagFileLocation"];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TeamName { get; set; }

        [Required]
        [StringLength(100)]
        public string TeamFlag { get; set; }

        public string AnimatedTeamFlag { get; set; }

        public string FlagFileLocation => _flagFileLocation + "/" + TeamFlag;

        public string AnimatedFlagFileLocation => _animatedFlagLocation + "/" + AnimatedTeamFlag;

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}