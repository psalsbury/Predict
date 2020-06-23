using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace Predict.Models
{
    public class Team
    {
        private readonly string _animatedFlagLocation;
        private readonly string _flagFileLocation;

        public Team()
        {
            _flagFileLocation = ConfigurationManager.AppSettings["FlagFileLocation"];
            _animatedFlagLocation = ConfigurationManager.AppSettings["AnimatedFlagFileLocation"];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required] [StringLength(50)] public string TeamName { get; set; }

        [Required] [StringLength(100)] public string TeamFlag { get; set; }

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