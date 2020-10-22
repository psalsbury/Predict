using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    [Table("EventPoolPlayerPositionHistory")]
    public class EventPoolPlayerPositionHistory
    {

        [Key, Column(Order = 0)]
        public short EventId { get; set; }

        [Key, Column(Order = 1)]
        public int PoolId { get; set; }

        [Key, Column(Order = 2)]
        public string PlayerId { get; set; }

        [Key, Column(Order = 3, TypeName = "Date")]
        public DateTime PositionDate { get; set; }

        [ForeignKey("EventId, PoolId, PlayerId")] public EventPoolPlayer EventPoolPlayer { get; set; }

        public short PoolPosition { get; set; }

        [Required]
        [Column(TypeName = "datetime2")] public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")] public DateTime ModifiedDateTime { get; set; }
    }
}