USE [LendingTracker]
GO

INSERT INTO [dbo].[Transactions]
           ([LenderId]
           ,[BorrowerId]
           ,[ItemId]
           ,[BorrowedAt]
           ,[ReturnedAt]
           ,[DueDate]
           ,[Status])
     VALUES
           ('4BC1E069-66BD-40FC-924F-5705BDAB7DE8'
           ,'9FBCD67F-FCF3-48F3-F73D-08DD1D224860'
           ,2003
           ,GETDATE()
           ,null
           ,DateAdd(DAY,7, GETDATE())
           ,'')
GO


