using Predict.Models;
using Predict.Enums;
using System.Collections.Generic;



namespace Predict.ViewModels
{
    public class QuizQuestionViewModel
    {
        public QuizQuestion QuizQuestion { get; set; }
        public List<QuizQuestionAnswer> QuizQuestionAnswers { get; set; }

    }
}