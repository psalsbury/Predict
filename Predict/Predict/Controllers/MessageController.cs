using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using PagedList;
using Predict.Models;
using Predict.ViewModels;


namespace Predict.Controllers
{
    public class MessageController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public ActionResult Index(int? id, int? page)
        {
            var pageSize = 5;
            var pageNumber = (page ?? 1);

            MessageReplyViewModel vm = new MessageReplyViewModel();
            var count = dbContext.Messages.Count();

            decimal totalPages = count / (decimal)pageSize;
            ViewBag.TotalPages = Math.Ceiling(totalPages);
            vm.Messages = dbContext.Messages
                .OrderBy(x => x.DatePosted).ToPagedList(pageNumber, pageSize);

            ViewBag.MessagesInOnePage = vm.Messages;
            ViewBag.PageNumber = pageNumber;

            if (id != null)
            {

                var replies = dbContext.Replies.Where(x => x.MessageId == id.Value).OrderByDescending(x => x.ReplyDateTime).ToList();
                if (replies != null)
                {
                    foreach (var rep in replies)
                    {
                        MessageReplyViewModel.MessageReply reply = new MessageReplyViewModel.MessageReply();
                        reply.MessageId = rep.MessageId;
                        reply.Id = rep.Id;
                        reply.ReplyMessage = rep.ReplyMessage;
                        reply.ReplyDateTime = rep.ReplyDateTime;
                        reply.MessageDetails = dbContext.Messages.Where(x => x.Id == rep.MessageId).Select(s => s.MessageToPost).FirstOrDefault();
                        reply.ReplyFrom = rep.ReplyFrom;
                        vm.Replies.Add(reply);
                    }

                }
                else
                {
                    vm.Replies.Add(null);
                }


                ViewBag.MessageId = id.Value;
            }

            return View(vm);
        }


        [HttpPost]
        [Authorize]
        public ActionResult PostMessage(MessageReplyViewModel vm)
        {
            var username = User.Identity.Name;
            string fullName = "";
            int msgid = 0;
            if (!string.IsNullOrEmpty(username))
            {
                var user = dbContext.Users.SingleOrDefault(u => u.UserName == username);
                fullName = user.Email;
            }
            Message messagetoPost = new Message();
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

            return RedirectToAction("Index", "Home", new { Id = msgid });
        }

        public ActionResult Create()
        {
            MessageReplyViewModel vm = new MessageReplyViewModel();

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReplyMessage(MessageReplyViewModel vm, int messageId)
        {
            var username = User.Identity.Name;
            string fullName = "";
            if (!string.IsNullOrEmpty(username))
            {
                var user = dbContext.Users.SingleOrDefault(u => u.UserName == username);
                fullName = user.Email;
            }
            if (vm.Reply.ReplyMessage != null)
            {
                Reply reply = new Reply();
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
            return RedirectToAction("Index", "Home", new { Id = messageId });

        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteMessage(int messageId)
        {
            Message messageToDelete = dbContext.Messages.Find(messageId);
            dbContext.Messages.Remove(messageToDelete);
            dbContext.SaveChanges();

            // also delete the replies related to the message
            var repliesToDelete = dbContext.Replies.Where(i => i.MessageId == messageId).ToList();
            if (repliesToDelete != null)
            {
                foreach (var rep in repliesToDelete)
                {
                    dbContext.Replies.Remove(rep);
                    dbContext.SaveChanges();
                    }
            }


            return RedirectToAction("Index", "Home");
        }
    }
}
