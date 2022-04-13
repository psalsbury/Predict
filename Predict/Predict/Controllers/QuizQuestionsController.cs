using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Predict.ViewModels;
using Microsoft.AspNet.Identity;
using Predict.Models;


namespace Predict.Controllers
{
    public class QuizQuestionsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public QuizQuestionsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult QuizTable()
        {

            var tableViewModels = _context.Database.SqlQuery<QuizTable>("dbo.spGetQuizLeague").ToList();
            return View(tableViewModels);

        }

        [HttpGet]
        public ActionResult AddQuizQuestion()
        {

            var quizQuestionViewModel = new QuizQuestionViewModel
            {
                QuizQuestion = new QuizQuestion(),
                QuizQuestionAnswers = new List<QuizQuestionAnswer>()
            };
            return View(quizQuestionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuizQuestion(QuizQuestionViewModel quizQuestionViewModel)
        {

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();
            var player = _context.Players.FirstOrDefault(a => a.Id == playerId);

            if (quizQuestionViewModel.QuizQuestion.Id != 0)
            {
                _context.QuizQuestionAnswers.RemoveRange(_context.QuizQuestionAnswers.Where(c => c.QuizQuestionId == quizQuestionViewModel.QuizQuestion.Id));
                _context.QuizQuestionPlayerAnswers.RemoveRange(_context.QuizQuestionPlayerAnswers.Where(c => c.QuizQuestionId == quizQuestionViewModel.QuizQuestion.Id));
            }
            else
            {
                quizQuestionViewModel.QuizQuestion.CreatedDateTime = DateTime.Now.ToUniversalTime();
                quizQuestionViewModel.QuizQuestion.PlayerId = playerId;
            }

            quizQuestionViewModel.QuizQuestion.ModifiedDateTime = DateTime.Now.ToUniversalTime();
            _context.QuizQuestions.AddOrUpdate(quizQuestionViewModel.QuizQuestion);

            if (quizQuestionViewModel.QuizQuestion.AnswerTypeId == Predict.Enums.QuestionType.MultipleChoice)
            {
                var correctAnswer = System.Convert.ToInt32(HttpContext.Request.Params.Get("ansMC"));
                var answerNumber = 0;
                var answer = "NOT NULL";
                while (answer != null)
                {
                    // loop around answers until no more left
                    answerNumber += 1;
                    answer = HttpContext.Request.Params.Get("ans" + answerNumber);
                    if (answer != null)
                    {
                        var quizQuestionAnswer = GetNewQuizQuestionAnswer(quizQuestionViewModel.QuizQuestion.Id);

                        quizQuestionAnswer.AnswerText = answer;
                        quizQuestionAnswer.IsCorrectAnswer = correctAnswer == answerNumber;
                        _context.QuizQuestionAnswers.Add(quizQuestionAnswer);
                    }
                }

            }
            else if (quizQuestionViewModel.QuizQuestion.AnswerTypeId == Predict.Enums.QuestionType.StraightAnswer)
            {
                var quizQuestionAnswer = GetNewQuizQuestionAnswer(quizQuestionViewModel.QuizQuestion.Id);

                quizQuestionAnswer.AnswerText = HttpContext.Request.Params.Get("ansSA"); ;
                quizQuestionAnswer.IsCorrectAnswer = true;
                _context.QuizQuestionAnswers.Add(quizQuestionAnswer);
            }
            else if (quizQuestionViewModel.QuizQuestion.AnswerTypeId == Predict.Enums.QuestionType.TrueOrFalse)
            {
                var quizQuestionAnswer = GetNewQuizQuestionAnswer(quizQuestionViewModel.QuizQuestion.Id);

                quizQuestionAnswer.AnswerText = "True";
                quizQuestionAnswer.IsCorrectAnswer = HttpContext.Request.Params.Get("ansTF") == "true";
                _context.QuizQuestionAnswers.Add(quizQuestionAnswer);
            }

            _context.SaveChanges();

            var email = new IdentityMessage
            {
                Body = player.PlayerName + " has submitted a " + (quizQuestionViewModel.QuizQuestion.Id == 0 ? "change to " : "") + "Quiz Question <br><br> "
                + "Question Text = " + quizQuestionViewModel.QuizQuestion.QuestionText,
                Subject = "New Quiz Question Submitted by  " + player.PlayerName,
                Destination = "pete@salsbury.co.uk"
            };
            Helper.Cache.SendEmail(email);

            if (quizQuestionViewModel.QuizQuestion.Id == 0)
            {
                Predict.Helper.SessionHelper.UpdatePlayerQuizQuestionCount(_context, Session, playerId, true);
            }

            return RedirectToAction("DisplayQuizQuestionList");
        }

        private QuizQuestionAnswer GetNewQuizQuestionAnswer(int quizQuestionId)
        {
            var quizQuestionAnswer = new QuizQuestionAnswer
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                QuizQuestionId = quizQuestionId
            };
            return quizQuestionAnswer;
        }

        [HttpGet]
        public ActionResult EditQuizQuestion(int id)
        {

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var quizQuestion = _context.QuizQuestions.FirstOrDefault(a => a.Id == id);

            if (quizQuestion == null)
                return RedirectToAction("Login", "Account");

            var quizQuestionViewModel = new QuizQuestionViewModel
            {
                QuizQuestion = quizQuestion,
                QuizQuestionAnswers = _context.QuizQuestionAnswers.Where(a => a.QuizQuestionId == id).ToList()
            };

            return View("AddQuizQuestion", quizQuestionViewModel);

        }


        [HttpGet]
        public ActionResult DisplayQuizQuestionList()
        {

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();

            var quizQuestions = _context.QuizQuestions.Where(a => a.PlayerId == playerId).ToList();

            return View(quizQuestions);
        }

        [HttpGet]
        public ActionResult DoQuiz()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var playerId = User.Identity.GetUserId();

            var quizQuestionsForQuizList = _context.Database.SqlQuery<QuizQuestionsForQuiz>(
                  "spGetQuestionsForQuiz @intPlayerId"
                  , new SqlParameter("@intPlayerId", playerId)).ToList();

            var quizQuestionsForQuizViewModel = new QuizQuestionsForQuizViewModel();
            quizQuestionsForQuizViewModel.QuizQuestionsForQuizList = quizQuestionsForQuizList;

            var listOfQuestionId = quizQuestionsForQuizList.Select(r => r.Id);
            quizQuestionsForQuizViewModel.QuizQuestionAnswerList = _context.QuizQuestionAnswers.Where(a => listOfQuestionId.Contains(a.QuizQuestionId)).ToList();

            return View(quizQuestionsForQuizViewModel);
        }

        [HttpPost]
        public ActionResult DoQuiz(QuizQuestionsForQuizViewModel quizQuestionsForQuizViewModel)
        {
            quizQuestionsForQuizViewModel.QuestionsAnswered = 0;
            quizQuestionsForQuizViewModel.Score = 0;

            var playerId = User.Identity.GetUserId();
            var newPlayerAnswers = false;
            var quiQuestionsIds = quizQuestionsForQuizViewModel.QuizQuestionsForQuizList.Select(a => a.Id).ToArray();

            // Get the existing answers for the questions presented.
            var quizQuestionPlayerAnswers = _context.QuizQuestionPlayerAnswers.Where(a => quiQuestionsIds.Contains(a.QuizQuestionId) && a.PlayerId == playerId).ToList();

            foreach (var quizQuestion in quizQuestionsForQuizViewModel.QuizQuestionsForQuizList)
            {
                quizQuestionsForQuizViewModel.QuestionsAnswered +=1;

                var playerAnswer = HttpContext.Request.Params.Get("question-" + quizQuestion.Id);

                if (quizQuestion.AnswerTypeId == Enums.QuestionType.MultipleChoice)
                {
                    // Get the answer that has been submitted by the player
                    quizQuestion.PlayerAnswerId = System.Convert.ToInt32(playerAnswer);
                    var quizQuestionAnswer = quizQuestionsForQuizViewModel.QuizQuestionAnswerList.FirstOrDefault(a => a.Id == quizQuestion.PlayerAnswerId);
                    quizQuestion.PlayerAnsweredCorrectly = quizQuestionAnswer.IsCorrectAnswer == true;
      
                }
                else
                {
                    // Get the text response submitted by the player
                    quizQuestion.PlayerAnswerText = playerAnswer;
                    var quizQuestionAnswer = quizQuestionsForQuizViewModel.QuizQuestionAnswerList.FirstOrDefault(a => a.QuizQuestionId == quizQuestion.Id);
                    
                    if(quizQuestion.AnswerTypeId == Enums.QuestionType.StraightAnswer)
                    {
                        quizQuestion.PlayerAnsweredCorrectly = quizQuestionAnswer.AnswerText == quizQuestion.PlayerAnswerText;
                    }
                    else
                    {
                        quizQuestion.PlayerAnsweredCorrectly = (quizQuestionAnswer.IsCorrectAnswer == true ? "True" : "False") == quizQuestion.PlayerAnswerText;
                    }
                }
                quizQuestionsForQuizViewModel.Score += quizQuestion.PlayerAnsweredCorrectly ?1 : 0;

                var answerGivenPreviously = quizQuestionPlayerAnswers.Any(a => a.QuizQuestionId == quizQuestion.Id);
                if(!answerGivenPreviously)
                {
                    var answerGiven = new QuizQuestionPlayerAnswer
                    {
                        QuizQuestionId = quizQuestion.Id,
                        PlayerId = playerId,
                        ModifiedDateTime = DateTime.UtcNow,
                        CreatedDateTime = DateTime.UtcNow,
                        IsCorrect = quizQuestion.PlayerAnsweredCorrectly,
                        AnswerGiven = playerAnswer
                    };
                    _context.QuizQuestionPlayerAnswers.Add(answerGiven);
                    newPlayerAnswers = true;
                }


            }

            if (newPlayerAnswers)
                _context.SaveChanges();

            quizQuestionsForQuizViewModel.IsResults = true;
            return View("DoQuiz", quizQuestionsForQuizViewModel);
        }

        [HttpGet]
        public ActionResult QuizLeagueTable()
        {
            return View("QuizLeagueTable");
        }
    }
}