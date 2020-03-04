using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    // This table links a player to a pool. (A pool is linked to 1 event)
    public class PoolPlayer
    {
        [Key, Column(Order = 0)]
        public int PoolId { get; set; }

        [Key, Column(Order = 1)]       
        [StringLength(128)]
        public string PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public Player Player { get; set; }

        [ForeignKey("PoolId")]
        public Pool Pool { get; set; }

        public DateTime? AdminApprovedDateTime { get; set; }

        public short FinalGoalMinutePrediction { get; set; }

        public short PoolPosition { get; set; }

        public short CorrectScore { get; set; } // Total score for all correct scores //

        public short CorrectResult { get; set; } // Total score for all correct results //

        public short WinMargin { get; set; } // Total score for all correct win margins //

        public short KoScore { get; set; } // Total score for all KO round scores //

        public short BonusScore { get; set; } // Total score for all bonus scores //

        public short TotalScore { get; set; } // Total score (all scores added from above) //

        [Required]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        public DateTime ModifiedDateTime { get; set; }

    }
}