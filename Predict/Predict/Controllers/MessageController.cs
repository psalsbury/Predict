using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();

        public ActionResult Index(int? id, int? page)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var pageSize = 5;
            var pageNumber = page ?? 1;

            var vm = new MessageReplyViewModel();
            var count = dbContext.Messages.Count();

            var totalPages = count / (decimal) pageSize;
            ViewBag.TotalPages = Math.Ceiling(totalPages);
            vm.Messages = dbContext.Messages
                .OrderBy(x => x.DatePosted).ToPagedList(pageNumber, pageSize);

            ViewBag.MessagesInOnePage = vm.Messages;
            ViewBag.PageNumber = pageNumber;

            if (id != null)
            {
                var replies = dbContext.Replies.Where(x => x.MessageId == id.Value)
                    .OrderByDescending(x => x.ReplyDateTime).ToList();
                if (replies != null)
                    foreach (var rep in replies)
                    {
                        var reply = new MessageReplyViewModel.MessageReply();
                        reply.MessageId = rep.MessageId;
                        reply.Id = rep.Id;
                        reply.ReplyMessage = rep.ReplyMessage;
                        reply.ReplyDateTime = rep.ReplyDateTime;
                        reply.MessageDetails = dbContext.Messages.Where(x => x.Id == rep.MessageId)
                            .Select(s => s.MessageToPost).FirstOrDefault();
                        reply.ReplyFrom = rep.ReplyFrom;
                        vm.Replies.Add(reply);
                    }
                else
                    vm.Replies.Add(null);


                ViewBag.MessageId = id.Value;
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostMessage(MessageReplyViewModel vm)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var username = User.Identity.Name;
            var fullName = "";
            var msgid = 0;
            if (!string.IsNullOrEmpty(username))
            {
                var user = dbContext.Users.SingleOrDefault(u => u.UserName == username);
                fullName = user.Email;
            }

            var messagetoPost = new Message();
            if (vm.Message.Subject != string.Empty && vm.Message.MessageToPost != string.Empty)
            {
                messagetoPost.DatePosted = DateTime.Now;
                messagetoPost.Subject = vm.Message.Subject;
                messagetoPost.MessageToPost = vm.Message.MessageToPost;
                messagetoPost.From = fullName;

                dbContext.Messages.Add(messagetoPost);
                dbContext.SaveChanges();
                msgid = messagetoPost.Id;
            }

            return RedirectToAction("Index", "Home", new {Id = msgid});
        }

        public ActionResult Create()
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var vm = new MessageReplyViewModel();

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReplyMessage(MessageReplyViewModel vm, int messageId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var username = User.Identity.Name;
            var fullName = "";
            if (!string.IsNullOrEmpty(username))
            {
                var user = dbContext.Users.SingleOrDefault(u => u.UserName == username);
                fullName = user.Email;
            }

            if (vm.Reply.ReplyMessage != null)
            {
                var reply = new Reply();
                reply.ReplyDateTime = DateTime.Now;
                reply.MessageId = messageId;
                reply.ReplyFrom = fullName;
                reply.ReplyMessage = vm.Reply.ReplyMessage;
                dbContext.Replies.Add(reply);
                dbContext.SaveChanges();
            }
            //reply to the message owner          - using email template

            var messageOwner = dbContext.Messages.Where(x => x.Id == messageId).Select(s => s.From).FirstOrDefault();
            var users = from user in dbContext.Users
                orderby user.Email
                select new
                {
                    FullName = user.Email,
                    UserEmail = user.Email
                };

            // var uemail = users.Where(x => x.FullName == messageOwner).Select(s => s.UserEmail).FirstOrDefault();
            // SendGridMessage replyMessage = new SendGridMessage();
            //replyMessage.From = new MailAddress(username);
            //replyMessage.Subject = "Reply for your message :" + dbContext.Messages.Where(i=>i.Id==messageId).Select(s=>s.Subject).FirstOrDefault();
            //replyMessage.Text = vm.Reply.ReplyMessage;


            //replyMessage.AddTo(uemail);


            //var credentials = new NetworkCredential("YOUR SENDGRID USERNAME", "PASSWORD");
            //var transportweb = new Web(credentials);
            //transportweb.DeliverAsync(replyMessage);
            return RedirectToAction("Index", "Home", new {Id = messageId});
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteMessage(int messageId)
        {
            // If user is not logged in redirect to the home page
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var messageToDelete = dbContext.Messages.Find(messageId);
            dbContext.Messages.Remove(messageToDelete);
            dbContext.SaveChanges();

            // also delete the replies related to the message
            var repliesToDelete = dbContext.Replies.Where(i => i.MessageId == messageId).ToList();
            if (repliesToDelete != null)
                foreach (var rep in repliesToDelete)
                {
                    dbContext.Replies.Remove(rep);
                    dbContext.SaveChanges();
                }


            return RedirectToAction("Index", "Home");
        }
    }
}