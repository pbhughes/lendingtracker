using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LendingTrackerApi.Models;

public partial class Borrower
{
    
    public Guid BorrowerId { get; set; }

    [Required]
    [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
    public Guid UserId { get; set; }

    [Required]
    public string Name { get; set; }

    public bool? IsEligible { get; set; }

    [Required]
    public string? BorrowerEmail { get; set; }

    [Required]    
    public string BorrowerSms { get; set; } = null!;

    [RegularExpression(@"^\+\d{1,3}$")]
    [MaxLength(5)]
    [AllowedValues("+1", "+44", "+91", "+49")]
    public string? CountryCode { get; set; }

    [JsonIgnore]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
