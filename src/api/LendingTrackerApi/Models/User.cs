using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LendingTrackerApi.Models;

public partial class User
{
    [Required(ErrorMessage = "Subject identifier from the authenticaiton platform is required")]
    [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    [RegularExpression(@"^\+\d{1,3}$")]
    [Required(ErrorMessage = "Country code is required")]
    [MaxLength(5)]
    [AllowedValues("+93", "+355", "+213", "+1", "+376", "+244", "+1", "+672", "+1", "+54", "+374", "+297", "+61", "+43", "+994", "+1", "+973", "+880", "+1", "+375", "+32", "+501", "+229", "+1", "+975", "+591", "+599", "+387", "+267", "+55", "+246", "+673", "+359", "+226", "+257", "+238", "+855", "+237", "+1", "+1", "+236", "+235", "+56", "+86", "+61", "+57", "+269", "+243", "+242", "+682", "+506", "+385", "+53", "+599", "+357", "+420", "+45", "+253", "+1", "+1", "+593", "+20", "+503", "+240", "+291", "+372", "+268", "+251", "+500", "+679", "+358", "+33", "+594", "+689", "+262", "+241", "+220", "+49", "+233", "+350", "+30", "+299", "+1", "+590", "+1", "+502", "+44", "+224", "+245", "+592", "+509", "+672", "+39", "+504", "+852", "+36", "+354", "+91", "+62", "+98", "+964", "+353", "+44", "+972", "+39", "+1", "+44", "+81", "+254", "+686", "+850", "+82", "+965", "+996", "+856", "+371", "+961", "+266", "+231", "+218", "+423", "+370", "+352", "+853", "+261", "+265", "+60", "+960", "+223", "+356", "+692", "+596", ErrorMessage = "Invalid country code")]
    public string CountryCode { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Borrower> Borrowers { get; set; } = new List<Borrower>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
