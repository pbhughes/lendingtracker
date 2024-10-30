USE LendingTracker

CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    LenderId INT NOT NULL,
    BorrowerId INT NOT NULL,
    ItemId INT NOT NULL,
    BorrowedAt DATETIME NOT NULL,
    ReturnedAt DATETIME NULL,  -- Null if the item hasn't been returned yet
    DueDate DATETIME NOT NULL, -- The date when the item is supposed to be returned
    Status NVARCHAR(50) DEFAULT 'Pending',  -- Pending, Returned, Overdue, etc.
    FOREIGN KEY (LenderId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    FOREIGN KEY (BorrowerId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    FOREIGN KEY (ItemId) REFERENCES Items(ItemId) ON DELETE CASCADE
);
