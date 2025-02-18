using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LendingTrackerApi.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    [DefaultValue(5)]
    public int MaxItems { get; set; }

    [DefaultValue(5)]
    public int MaxBorrowers { get; set; }

    public virtual ICollection<Borrower> Borrowers { get; set; } = new List<Borrower>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
