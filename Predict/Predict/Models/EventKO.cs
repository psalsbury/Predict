using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    [Table("EventsKo")]
    public class EventKo
    {
        [Key] [Column(Order = 0)] public short EventId { get; set; }

        [ForeignKey("EventId")] public Event Event { get; set; }

        [Column(Order = 1)] public short? KoStageFirstRoundQty { get; set; }

        public int? WinningTeamId { get; set; }

        [ForeignKey("WinningTeamId")] public Team WinningTeam { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}