using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class EventPool
    {

        [Key] [Column(Order = 0)]
        public short EventId { get; set; }

        [Key] [Column(Order = 1)]
        public int PoolId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("PoolId")]
        public Pool Pool { get; set; }

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    

}
}