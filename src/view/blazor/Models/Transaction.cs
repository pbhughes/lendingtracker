using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LendingView.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        [Required]
        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid LenderId { get; set; }

        [Required]
        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid BorrowerId { get; set; }

        [Required]
        public int ItemId { get; set; }

        public DateTime BorrowedAt { get; set; }

        public DateTime? ReturnedAt { get; set; }

        public DateTime DueDate { get; set; }

        public string? Status { get; set; }

        public virtual Borrower Borrower { get; set; } = null!;
        [JsonIgnore]
        public virtual Item Item { get; set; } = null!;

        public virtual User Lender { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
