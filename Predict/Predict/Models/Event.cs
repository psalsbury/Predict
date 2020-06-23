using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    // A single event or competition. 
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [Required] [StringLength(100)] public string EventName { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime EventStartDateTime { get; set; }

        public int? DefaultPoolId { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}