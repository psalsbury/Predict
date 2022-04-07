using System;
using System.ComponentModel.DataAnnotations;


namespace Predict.ViewModels
{
    public class JokeTable
    {
        public long Position { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string JokeText { get; set; }

        public decimal AvgRating { get; set; }

        public Int32 NbrRatings { get; set; }

    }
}