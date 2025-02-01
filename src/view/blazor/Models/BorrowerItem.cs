using System;

namespace LendingView.Models
{
    public class BorrowerItem
    {
        public Guid BorrowerId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public string ItemName { get; set; }
        public Guid LenderId { get; set; }

        public Guid TransactionId { get; set; }

        public DateTime ReturnDate { get; set; }

    }
}
