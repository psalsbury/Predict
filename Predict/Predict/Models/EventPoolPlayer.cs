using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    // This table links a player to a pool. (A pool is linked to 1 event)
    public class EventPoolPlayer
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
        
        public DateTime? AdminApprovedDateTime { get; set; }

        public short PoolPosition { get; set; }

        public short CorrectScore { get; set; } // Total score for all correct scores //

        public short CorrectResult { get; set; } // Total score for all correct results //

        public short WinMargin { get; set; } // Total score for all correct win margins //

        public short KoScore { get; set; } // Total score for all KO round scores //

        public short BonusScore { get; set; } // Total score for all bonus scores //

        public short TotalScore { get; set; } // Total score (all scores added from above) //

        [Column(TypeName = "bit")]
        public bool Enabled { get; set; }

        [Required] public DateTime CreatedDateTime { get; set; }

        [Required] public DateTime ModifiedDateTime { get; set; }
    }
}