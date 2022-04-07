using System;
using System.ComponentModel.DataAnnotations;

namespace Predict.ViewModels
{
    public class QuizTable
    {
        public long Position { get; set; }

        [Required]
        public string PlayerName { get; set; }

        public int? SupportTeamId { get; set; }

        [Required]
        public Int32 QuestionsAvailable { get; set; }

        [Required]
        public Int32 QuestionsAnswered { get; set; }

        [Required]
        public Int32 AnsweredCorrectly { get; set; }

        [Required]
        public Int32 Percent { get; set; }



    }
}