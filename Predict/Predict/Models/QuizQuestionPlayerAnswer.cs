using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class QuizQuestionPlayerAnswer
    {
        [Key, Column(Order = 0)]
        public int QuizQuestionId { get; set; }

        [StringLength(128)]
        [ForeignKey("Player")]
        [Key, Column(Order = 1)]
        public string PlayerId { get; set; }

        public Player Player { get; set; }

        [StringLength(100)]
        [Required]
        public string AnswerGiven { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}