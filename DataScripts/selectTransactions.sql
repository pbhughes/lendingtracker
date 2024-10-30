USE LendingTracker

SELECT 
    I.ItemId, 
    I.ItemName, 
    I.Description, 
    I.Category, 
    I.IsAvailable 
FROM Items I
WHERE I.OwnerId = 1;  -- Replace with the UserId of the owner
