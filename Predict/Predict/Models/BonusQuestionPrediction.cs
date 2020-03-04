using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class BonusQuestionPrediction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int BonusQuestionId { get; set; }

        [ForeignKey("BonusQuestionId")]
        public BonusQuestion BonusQuestion { get; set; }

        [Required]
        [StringLength(128)]
        public string PlayerId { get; set; }

        [StringLength(50)]
        public string PredictedAnswer { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}