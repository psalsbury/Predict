using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Predict.Models
{
    public class ForumMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [ForeignKey("ForumTopic")]
        public long ForumTopicId { get; set; }

        public ForumTopic ForumTopic { get; set; }

        [StringLength(128)]
        [ForeignKey("Player")]
        [Required]
        public string PlayerId { get; set; }

        public Player Player { get; set; }

        public long ?ReplyToForumMessageId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}