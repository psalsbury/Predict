using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class BonusQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required] public short EventId { get; set; }

        [Required] [StringLength(200)] public string Question { get; set; }

        [Required] public int Score { get; set; }

        [StringLength(50)] public string Answer { get; set; }

        [ForeignKey("EventId")] public Event Event { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ToBeAnsweredByDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        public bool DatePassed => ToBeAnsweredByDateTime < DateTime.Now.ToUniversalTime();
    }
}