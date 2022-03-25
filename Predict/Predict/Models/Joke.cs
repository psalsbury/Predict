using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class Joke
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int Id { get; set; }

        [StringLength(128)]
        [ForeignKey("Player")]
        [Required]
        public string PlayerId { get; set; }

        public Player Player { get; set; }

        [Required]
        [StringLength(500)]
        public string JokeText { get; set; }

        [Required]
        [StringLength(500)]
        public string JokePunchline { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}