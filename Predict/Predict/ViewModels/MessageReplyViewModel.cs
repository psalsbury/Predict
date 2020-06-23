using System;
using System.Collections.Generic;
using PagedList;
using Predict.Models;

namespace Predict.ViewModels
{
    public class MessageReplyViewModel
    {
        public Reply Reply { get; set; }

        public Message Message { get; set; }

        public List<MessageReply> Replies { get; set; } = new List<MessageReply>();

        public IPagedList<Message> Messages { get; set; }

        public class MessageReply
        {
            public int Id { get; set; }
            public int MessageId { get; set; }
            public string MessageDetails { get; set; }
            public string ReplyFrom { get; set; }

            public string ReplyMessage { get; set; }
            public DateTime ReplyDateTime { get; set; }
        }
    }
}