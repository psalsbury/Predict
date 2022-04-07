using System.ComponentModel.DataAnnotations;

namespace Predict.Enums
{
        public enum QuestionType
        {
            [Display(Name = "Multiple Choice")]
            MultipleChoice = 1,
            [Display(Name = "Straight Answer")]
            StraightAnswer = 2,
            [Display(Name = "True Or False")]
            TrueOrFalse = 3
    }
}