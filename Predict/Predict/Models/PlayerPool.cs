using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class PlayerPool
    {

        [Key, Column(Order = 0)]        
        public string PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public Player Player { get; set; }

        [Key, Column(Order = 1)]        
        public int PoolId { get; set; }

        [ForeignKey("PoolId")]
        public Pool Pool { get; set; }

        public DateTime? AdminApprovedDateTime { get; set; }

        public short FinalGoalMinute { get; set; }

        public short PoolPosition { get; set; }

        public short CorrectScore { get; set; }

        public short CorrectResult { get; set; }

        public short WinMargin { get; set; }

        public short KoScore { get; set; }

        public short BonusScore { get; set; }

        public short TotalScore { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        public DateTime ModifiedDateTime { get; set; }

    }
}