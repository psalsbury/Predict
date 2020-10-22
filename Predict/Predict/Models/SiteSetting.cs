using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        [Column(TypeName = "datetime2")] public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")] public DateTime ModifiedDateTime { get; set; }


    }
}