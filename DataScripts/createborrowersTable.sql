USE LendingTracker

CREATE TABLE Borrowers (
    BorrowerId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    BorrowingHistory NVARCHAR(500),   -- Optional: You can store the borrowing history or other details here
    IsEligible BIT DEFAULT 1,         -- Optional: You can track if a borrower is eligible to borrow items
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
