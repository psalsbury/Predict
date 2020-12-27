using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class LeagueEventGeneration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public short Id { get; set; }

        public short LeagueId { get; set; }

        [ForeignKey("LeagueId")]
        public League League { get; set; }

        public short GenerationFrequencyId { get; set; }

        [Column(TypeName = "bit")]
        public Boolean Enabled { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}