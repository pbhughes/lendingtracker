USE [LendingTracker]
GO

INSERT INTO [dbo].[Message]
           ( [Id]
           ,[Method]
           ,[Direction]
           ,[Text]
           ,[Phone]
           ,[ItemId]
           ,[TransactionId]
		   )
     VALUES
           ( NEWID()
           ,'sms'
           ,1
           ,'hello from borrower'
           ,'+12707049633'
           ,null
           ,'CC95E079-927D-4F6F-A287-19CDB30BE37A'
		   )
GO


