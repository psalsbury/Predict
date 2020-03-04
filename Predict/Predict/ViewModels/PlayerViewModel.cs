using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Predict.Models;

namespace Predict.ViewModels
{
    public class PlayerViewModel
    {
        public Player Player { get; set; }
        public ApplicationUser AspNetUser { get; set; }

    }
}