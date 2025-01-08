using System;

namespace LendingView.Models
{
    public class BorrowerItems
    {
        public Guid BorrowerId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public string ItemName { get; set; }
        public Guid LenderId { get; set; }
        
    }
}
