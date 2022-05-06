using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Predict.Models;

namespace Predict.Controllers
{
    public class ForumTopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 25;

        public ForumTopicsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: ForumTopics
        [HttpGet]
        public ActionResult ForumIndex(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            var forumTopics = _context.ForumTopics
                .Include(p => p.Player)
                .OrderByDescending(a => a.CreatedDateTime).ToList();

            return View(forumTopics.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult ForumTopicIndex(long forumTopicId)
        {
            var forumMessages = _context.ForumMessages
                .Include(p => p.Player)
                .Include(t => t.ForumTopic)
                .Where(a => a.ForumTopicId == forumTopicId)
                .OrderBy(a => a.CreatedDateTime).ToList();

            return View(forumMessages);
        }

        [HttpGet]
        public ActionResult NewForumReply(long forumTopicId, long forumMessageId)
        {
            var forumMessage = new ForumMessage
            {
                ForumTopicId = forumTopicId,
                ForumTopic = _context.ForumTopics.FirstOrDefault(a => a.Id == forumTopicId),
                ReplyToForumMessageId = forumMessageId,
                PlayerId = User.Identity.GetUserId()
        };

            return View(forumMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult SaveReply(ForumMessage forumMessage)
        {
            if (!CheckUserIsValid())
            {
                return RedirectToAction("Index", "Home");
            }

            forumMessage.CreatedDateTime = DateTime.UtcNow;
            forumMessage.ModifiedDateTime = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                return View("NewForumReply", forumMessage);
            }

            forumMessage.ForumTopic = null;
            _context.ForumMessages.AddOrUpdate(forumMessage);
            _context.SaveChanges();
            return RedirectToAction("ForumTopicIndex", "ForumTopics", new  { @forumTopicId = forumMessage.ForumTopicId});
        }

        [HttpGet]
        public ActionResult NewForumTopic()
        {
            var playerId = User.Identity.GetUserId();
            var forumMessage = new ForumMessage
            {
                ForumTopic = new ForumTopic 
                { 
                    PlayerId = playerId
                }
                , PlayerId = playerId
            };
            return View(forumMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult SaveNewTopic(ForumMessage forumMessage)
        {

            if (!CheckUserIsValid())
            {
                return RedirectToAction("Index", "Home");
            }

            forumMessage.CreatedDateTime = DateTime.UtcNow;
            forumMessage.ModifiedDateTime = DateTime.UtcNow;
            forumMessage.ForumTopic.CreatedDateTime = DateTime.UtcNow;
            forumMessage.ForumTopic.ModifiedDateTime = DateTime.UtcNow;

            if (!ModelState.IsValid)
            {
                return View("NewForumTopic", forumMessage);
            }

            forumMessage.ReplyToForumMessageId = null;

            _context.ForumTopics.AddOrUpdate(forumMessage.ForumTopic);
            _context.ForumMessages.AddOrUpdate(forumMessage);
            _context.SaveChanges();

            return RedirectToAction("ForumIndex", "ForumTopics");
        }

        private bool CheckUserIsValid()
        {
            if (!User.Identity.IsAuthenticated) return false;
            return true;
        }

    }
}