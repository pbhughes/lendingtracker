using System.ComponentModel.DataAnnotations;

namespace LendingTrackerApi.Models
{
    public class StandardMessage
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(166)]
        public string Text { get; set; }
    }
}
