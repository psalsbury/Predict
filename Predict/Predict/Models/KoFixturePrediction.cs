using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class KoFixturePrediction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [Required] public int KoFixtureId { get; set; }

        [ForeignKey("KoFixtureId")] public KoFixture KoFixture { get; set; }

        [Required] [StringLength(128)] public string PlayerId { get; set; }

        public int? Team1Id { get; set; }

        public int? Team2Id { get; set; }

        [ForeignKey("Team1Id")] public Team Team1 { get; set; }

        [ForeignKey("Team2Id")] public Team Team2 { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}