using System;
using System.ComponentModel.DataAnnotations;

namespace Predict.Models
{
    public class SiteSetting
    {
        [Key]
        [Required]
        [StringLength(100)]
        public string SettingName { get; set; }

        [Required]
        [StringLength(200)]
        public string SettingValue { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }


    }
}