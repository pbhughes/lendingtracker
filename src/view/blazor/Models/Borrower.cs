using LendingView.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace LendingView.Models
{
    public class Borrower
    {
        public Borrower()
        {
            BorrowerId = Guid.NewGuid();
        }
        private string _trash = null;

        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid BorrowerId { get; set; }

        [Required]
        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid UserId { get; set; }

        public bool? IsEligible { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? BorrowerEmail { get; set; }


        [Required]
        public string BorrowerSms { get; set; } = null;

        [ValidInternationalPhone(ErrorMessage = "Must provide a valid phone number")]
        public string CombinedSms
        {
            get
            {
                return $"{CountryCode} {BorrowerSms}";
            }
            set
            {
                _trash = value; 
            }
        }

        [Required]
        public string Name {  get; set; }   

        [RegularExpression(@"^\+\d{1,3}$")]
        [MaxLength(4)]
        public string? CountryCode { get; set; }
    }
}
