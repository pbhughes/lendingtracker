using LendingView.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public string Name { get; set; }

        [RegularExpression(@"^\+\d{1,3}$")]
        [MaxLength(4)]
        public string? CountryCode { get; set; }

        public virtual List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class BorrowerItem
    {
        public Guid BorrowerId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public string ItemName { get; set; }
        public Guid LenderId { get; set; }

        public Guid TransactionId { get; set; }

        public DateTime ReturnDate { get; set; }

    }

    public class Item
    {
        public int ItemId { get; set; }

        [RegularExpression("^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        [Required]
        public Guid OwnerId { get; set; }

        [MaxLength(100)]
        [Required]
        public string ItemName { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Transaction Transaction { get; set; }

        [Url]
        public string? StoreLink { get; set; }

        [Url]
        public string? ImageLink { get; set; }
    }

    public class User
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int MaxItems { get; set; }
        public int MaxBorrowers { get; set; }
    }

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

        public DateTime? ReturnDate { get; set; }
    }

    public class Message
    {
        public string Id { get; set; }
        public string Method { get; set; }
        public int Direction { get; set; }
        public string Text { get; set; }
        public string MessageDate { get; set; }
        public string Phone { get; set; }
        public int? ItemId { get; set; }
        public string TransactionId { get; set; }
    }
    public class Country 
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public string Alpha2Code { get; set; }
        public string FlagUrl { get; set; } // URL for the flag icon
    }

    public static class CountryData
    {
        public static List<Country> GetCountries() => new List<Country>
        {
            new Country { Name = "United States / Canada", Code = "+1", FlagUrl = "https://flagcdn.com/us.svg", Alpha2Code= "US"  },
            new Country { Name = "United Kingdom", Code = "+44", FlagUrl = "https://flagcdn.com/gb.svg", Alpha2Code = "UK" },
            new Country { Name = "India", Code = "+91", FlagUrl = "https://flagcdn.com/in.svg" , Alpha2Code = "IN"},
            new Country { Name = "Germany", Code = "+49", FlagUrl = "https://flagcdn.com/de.svg", Alpha2Code = "DE" }
            // Add more countries as needed
        };
    }
    public class FileUploadResponse
    {
        public string Url { get; set; }
    }
}
