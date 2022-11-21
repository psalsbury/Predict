using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class PoolChat
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [ForeignKey("Pool")]
        public int PoolId { get; set; }

        public Pool Pool { get; set; }

        [StringLength(128)]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }

        public Player Player { get; set; }

        [Required]
        [StringLength(100)]
        public string Message { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

    }
}