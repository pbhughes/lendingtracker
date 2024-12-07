using System.ComponentModel.DataAnnotations;
using System;

namespace LendingView.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        public string ItemName { get; set; } = null!;

        public string? Description { get; set; }

        public bool? IsAvailable { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
