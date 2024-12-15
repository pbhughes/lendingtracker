using System;
using System.ComponentModel.DataAnnotations;

namespace LendingView.Models
{
    public class Borrower
    {
        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid BorrowerId { get; set; }

        [Required]
        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid UserId { get; set; }

        public bool? IsEligible { get; set; }

        [Required]
        public string? BorrowerEmail { get; set; }

        [Required]
        public string BorrowerSms { get; set; } = null!;

        [Required]
        public string Name {  get; set; }   

        [RegularExpression(@"^\+\d{1,3}$")]
        [MaxLength(4)]
        public string? CountryCode { get; set; }
    }
}
