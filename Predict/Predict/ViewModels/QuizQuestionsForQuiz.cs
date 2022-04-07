using System;
using System.ComponentModel.DataAnnotations;

namespace Predict.ViewModels
{
    public class QuizQuestionsForQuiz
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public string PlayerID { get; set; }

        [Required]
        public string QuestionSubmittedBy { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public Predict.Enums.QuestionType AnswerTypeId { get; set; }
        
        public int PlayerAnswerId { get; set; }

        public string PlayerAnswerText{ get; set; }

        public bool PlayerAnsweredCorrectly { get; set; }

    }
}