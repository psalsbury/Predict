using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class PoolPlayer
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

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }

    }
}