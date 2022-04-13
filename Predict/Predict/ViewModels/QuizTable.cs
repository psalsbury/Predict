using System;
using System.ComponentModel.DataAnnotations;

namespace Predict.ViewModels
{
    public class QuizTable
    {

        [Required]
        public string PlayerName { get; set; }

        public string TeamFlag { get; set; }

        [Required]
        public Int32 QuestionsAnswered { get; set; }

        [Required]
        public Int32 QuestionsCorrect { get; set; }

        [Required]
        public Int32 QuestionsAvailable { get; set; }

        [Required]
        public Decimal Score { get; set; }



    }
}