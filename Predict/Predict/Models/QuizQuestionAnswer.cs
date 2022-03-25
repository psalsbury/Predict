using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class QuizQuestionAnswer
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey("QuizQuestion")]
        public int QuizQuestionId { get; set; }

        public QuizQuestion QuizQuestion { get; set; }

        [StringLength(100)]
        public string AnswerText { get; set; }

        public bool IsCorrectAnswer { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

    }
}