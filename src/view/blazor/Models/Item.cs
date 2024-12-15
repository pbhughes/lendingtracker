using System.ComponentModel.DataAnnotations;
using System;

namespace LendingView.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        [Required]
        public Guid OwnerId { get; set; }

        [MaxLength(100)]
        [Required]
        public string ItemName { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
