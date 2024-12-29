using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LendingTrackerApi.Models;

public partial class Borrower
{
    public Guid BorrowerId { get; set; }

    public Guid UserId { get; set; }

    public bool? IsEligible { get; set; }

    public string BorrowerEmail { get; set; } = null!;

    public string BorrowerSms { get; set; } = null!;

    public string? CountryCode { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
