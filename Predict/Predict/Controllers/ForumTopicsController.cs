using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Predict.Models;
using Predict.ViewModels;

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
        public ActionResult ForumIndex(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = PAGE_SIZE;

            var forumTopics = _context.ForumTopics
                .Include(p => p.Player)
                .OrderByDescending(a => a.CreatedDateTime).ToList();

            return View(forumTopics.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ForumTopicIndex(long forumTopicId)
        {
            var forumMessages = _context.ForumMessages
                .Include(p => p.Player)
                .Include(t => t.ForumTopic)
                .Where(a => a.ForumTopicId == forumTopicId)
                .OrderBy(a => a.CreatedDateTime).ToList();

            return View(forumMessages);
        }

        public ActionResult NewForumReply(long forumTopicId, long forumMessageId)
        {
            var forumMessage = new ForumMessage
            {
                ForumTopicId = forumTopicId,
                ForumTopic = _context.ForumTopics.FirstOrDefault(a => a.Id == forumTopicId),
                ReplyToForumMessageId = forumMessageId
            };

            return View(forumMessage);
        }

        public ActionResult SaveReply(ForumMessage forumMessage)
        {
            if (!CheckUserIsValid())
            {
                return RedirectToAction("Index", "Home");
            }

            forumMessage.CreatedDateTime = DateTime.UtcNow;
            forumMessage.ModifiedDateTime = DateTime.UtcNow;
            forumMessage.PlayerId = User.Identity.GetUserId();
            _context.ForumMessages.AddOrUpdate(forumMessage);
            _context.SaveChanges();
            return RedirectToAction("ForumIndex", "ForumTopics");
        }

        public ActionResult NewForumTopic()
        {
            var forumMessage = new ForumMessage
            {
                ForumTopic = new ForumTopic()
            };
            return View(forumMessage);
        }

        public ActionResult SaveNewTopic(ForumMessage forumMessage)
        {

            if (!CheckUserIsValid())
            {
                return RedirectToAction("Index", "Home");
            }

            forumMessage.ForumTopic.CreatedDateTime= DateTime.UtcNow;
            forumMessage.ForumTopic.ModifiedDateTime = DateTime.UtcNow;
            forumMessage.ForumTopic.PlayerId = User.Identity.GetUserId();

            forumMessage.CreatedDateTime = DateTime.UtcNow;
            forumMessage.ModifiedDateTime = DateTime.UtcNow;
            forumMessage.PlayerId = User.Identity.GetUserId();
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