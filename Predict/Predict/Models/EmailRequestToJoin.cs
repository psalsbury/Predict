using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class EmailRequestToJoin
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Event")]
        public short EventId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Pool")]
        public int PoolId { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(128)]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }

        public Event Event { get; set; }

        public Player Player { get; set; }

        public Pool Pool { get; set; }

        public short StatusId { get; set; } // 1 = ready to send, 2 = sent, 3 = not sent as no longer in pool, 4 not sent as already joined

        [Column(TypeName = "datetime2")]
        public DateTime? SentDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }



    }
}