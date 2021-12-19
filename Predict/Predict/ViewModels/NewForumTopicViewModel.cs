using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class NewForumTopicViewModel
    {
        public ForumTopic ForumTopic { get; set; }

        public ForumMessage ForumMessage { get; set; }
    }
}