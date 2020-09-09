using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class SiteSetting
    {
        [Key]
        [Required]
        [StringLength(25)]
        public string SittingName { get; set; }

        [Required]
        [StringLength(200)]
        public string SettingValue { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }


    }
}