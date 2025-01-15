using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LendingTrackerApi.Models;

public partial class Transaction
{
    public Guid TransactionId { get; set; }

    public Guid LenderId { get; set; }

    public Guid BorrowerId { get; set; }

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

    public DateTime? ReturnDate { get; set; }
}
