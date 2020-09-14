using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    [Table("PoolPlayerPositionHistory")]
    public class PoolPlayerPositionHistory
    {

        [Key, Column(Order = 0)]
        public short EventId { get; set; }

        [Key, Column(Order = 1)]
        public int PoolId { get; set; }

        [Key, Column(Order = 2)]
        public string PlayerId { get; set; }

        [Key, Column(Order = 3, TypeName = "Date")]
        public DateTime PositionDate { get; set; }

        [ForeignKey("EventId, PoolId, PlayerId")] public PoolPlayer PoolPlayer { get; set; }

        public short PoolPosition { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }
    }
}