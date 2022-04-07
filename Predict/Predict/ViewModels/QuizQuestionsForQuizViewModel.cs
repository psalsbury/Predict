using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class QuizQuestionsForQuizViewModel
    {

        public List<QuizQuestionsForQuiz> QuizQuestionsForQuizList { get; set; }

        public List<QuizQuestionAnswer> QuizQuestionAnswerList { get; set; }

        public int Score { get; set; }

        public int QuestionsAnswered { get; set; }

        public bool IsResults { get; set; }

    }
}