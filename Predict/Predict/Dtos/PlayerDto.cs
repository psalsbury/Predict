using System;
using System.ComponentModel.DataAnnotations;

namespace Predict.Dtos
{
    public class PlayerDto
    {
        [StringLength(128)] public string Id { get; set; }

        public string PlayerName { get; set; }

        public string DisplayName { get; set; }

        public string SupportTeamId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public DateTime? EmailConfirmedDateTime { get; set; }
    }
}