using System.ComponentModel.DataAnnotations;
using System;

namespace LendingView.Models
{
    public class User
    {
        [Required(ErrorMessage = "Subject identifier from the authenticaiton platform is required")]
        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^\+\d{1,3}$")]
        [Required(ErrorMessage = "Country code is required")]
        [MaxLength(5)]
        public string? CountryCode { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
