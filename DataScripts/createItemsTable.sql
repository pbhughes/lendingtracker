USE LendingTracker

CREATE TABLE Items (
    ItemId INT PRIMARY KEY IDENTITY(1,1),
    ItemName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    OwnerId INT NOT NULL,
    IsAvailable BIT DEFAULT 1,  -- 1: Available, 0: Not Available
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OwnerId) REFERENCES Users(UserId) ON DELETE CASCADE
);
