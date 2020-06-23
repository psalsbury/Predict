using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Predict.Models
{
    public class Pool
    {
        // TODO Add tags to change the name displayed on the form 

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] [StringLength(50)] public string PoolName { get; set; }

        public string AdminPlayerId { get; set; }

        [ForeignKey("AdminPlayerId")] public Player AdminPlayer { get; set; }

        public short EventId { get; set; }

        [ForeignKey("EventId")] public Event Event { get; set; }

        [StringLength(50)]
        [Display(Name = "Join Code")]
        public string JoinCode { get; set; }

        [StringLength(1000)]
        [Display(Name = "Initial Info")]
        public string InitialInfo { get; set; }

        [StringLength(1000)]
        [Display(Name = "Member Info")]
        public string MemberInfo { get; set; }

        [Required]
        [Display(Name = "Freeze Predictions")]
        public bool FreezePredictions { get; set; }

        [Range(0, 100000,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public float? EntryFee { get; set; }

        [Range(0, 100,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "First Percent")]
        public float? FirstPercent { get; set; }

        [Range(0, 100,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Second Percent")]
        public float? SecondPercent { get; set; }

        [Range(0, 100,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Third Percent")]
        public float? ThirdPercent { get; set; }

        [Range(0, 100,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Non Prize Percent")]
        public float? NonPrizePercent { get; set; }

        [Required]
        [Display(Name = "Email Notifications")]
        public bool EmailNotifications { get; set; }

        [Display(Name = "Correct Score Points")]
        public int CorrectScorePoints { get; set; }

        [Display(Name = "Correct Result Points")]
        public int CorrectResultPoints { get; set; }

        [Display(Name = "Win Margin Points")] public int WinMarginPoints { get; set; }

        public int KoLast16Points { get; set; }
        public int KoLast8Points { get; set; }
        public int KoLast4Points { get; set; }
        public int KoLast2Points { get; set; }
        public int KoLast1Points { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDateTime { get; set; }
    }
}