USE LendingTracker

--seed test data

-- Insert a Lender
INSERT INTO Users (FullName, Email, PhoneNumber, Address) 
VALUES ('John Doe', 'john@example.com', '123-456-7890', '123 Main St');

-- Insert a Borrower
INSERT INTO Users (FullName, Email, PhoneNumber, Address) 
VALUES ('Jane Smith', 'jane@example.com', '987-654-3210', '456 Elm St');


-- Insert an item owned by John Doe (UserId 1)
INSERT INTO Items (ItemName, Description, Category, OwnerId) 
VALUES ('Power Drill', 'Cordless power drill with battery', 'Tools', 1);

-- Insert another item owned by John Doe
INSERT INTO Items (ItemName, Description, Category, OwnerId) 
VALUES ('Camping Tent', '4-person camping tent', 'Outdoor', 1);

-- Jane Smith (UserId 2) borrows the Power Drill (ItemId 1) from John Doe (UserId 1)
INSERT INTO Transactions (LenderId, BorrowerId, ItemId, BorrowedAt, DueDate, Status) 
VALUES (1, 2, 1, GETDATE(), DATEADD(DAY, 7, GETDATE()), 'Pending');
