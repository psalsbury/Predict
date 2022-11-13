using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class PoolChat
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Pool")]
        public int PoolId { get; set; }

        public Pool Pool { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(128)]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }

        public Player Player { get; set; }

        [Required]
        [StringLength(100)]
        public string Message { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        [Key, Column(Order = 2, TypeName = "datetime2")]
        [Required]
        public DateTime CreatedDateTime { get; set; }

    }
}