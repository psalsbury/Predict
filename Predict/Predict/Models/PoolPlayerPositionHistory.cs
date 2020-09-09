using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    [Table("PoolPlayerPositionHistory")]
    public class PoolPlayerPositionHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int PoolId { get; set; }

        public short EventId { get; set; }

        public string PlayerId { get; set; }

        [ForeignKey("PoolId, PlayerId, EventId")] public PoolPlayer PoolPlayer { get; set; }

        [Column(TypeName = "Date")] public DateTime PositionDate { get; set; }

        public short PoolPosition { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }
    }
}