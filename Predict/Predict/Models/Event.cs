using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    // A single event or competition. 
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public short Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Comp Name")]
        public string EventName { get; set; }

        [StringLength(100)]
        [Display(Name = "Comp Description")]
        public string EventDescription { get; set; }

        public int DefaultPoolId { get; set; }

        public string CreatedByPlayerId { get; set; }

        [ForeignKey("CreatedByPlayerId")] public Player CreatedByPlayer { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartDateTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EndDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        public bool EventStarted => StartDateTime < DateTime.UtcNow;
    }
}