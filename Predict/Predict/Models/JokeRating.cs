using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class JokeRating
    {
        [Key, Column(Order = 0)]
        [Required]
        public int JokeId { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(128)]
        [ForeignKey("Player")]
        [Required]
        public string PlayerId { get; set; }

        public Player Player { get; set; }

        [Required]
        [Range(1,10)]
        public Int16 PlayerJokeRating { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

    }
}