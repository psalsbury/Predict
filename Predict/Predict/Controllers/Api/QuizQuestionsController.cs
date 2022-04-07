using System;
using System.Linq;
using System.Web.Http;
using Predict.Models;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers.Api
{
    public class QuizQuestionsController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public QuizQuestionsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("api/QuizQuestions/delete/{Id}")]
        public IHttpActionResult Delete(int Id)
        {

            if (!User.Identity.IsAuthenticated)
                return BadRequest();

            var playerId = User.Identity.GetUserId();

            var quizQuestion = _context.QuizQuestions.SingleOrDefault(p => p.Id == Id);
            if (quizQuestion == null)
                return BadRequest("Quiz Question does not exist");

            if (quizQuestion.PlayerId != playerId)
                return BadRequest("Invalid user");

            _context.QuizQuestionPlayerAnswers.RemoveRange(_context.QuizQuestionPlayerAnswers.Where(a => a.QuizQuestionId == quizQuestion.Id));
            _context.QuizQuestionAnswers.RemoveRange(_context.QuizQuestionAnswers.Where(a => a.QuizQuestionId == quizQuestion.Id));

            _context.QuizQuestions.Remove(quizQuestion);
            _context.SaveChanges();

            return Ok();
        }
    }
}
