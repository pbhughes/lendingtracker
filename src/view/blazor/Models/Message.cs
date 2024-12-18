using System;

namespace LendingView.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Method { get; set; } = null!;

        public int Direction { get; set; }

        public string Text { get; set; } = null!;

        public string MessageDate { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public int? ItemId { get; set; }

        public Guid TransactionId { get; set; }

        public virtual Item? Item { get; set; }

        public virtual Transaction Transaction { get; set; } = null!;
    }
}
