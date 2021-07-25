/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET IDENTITY_INSERT [dbo].[ApplicationRole] ON
DECLARE @ROLE_USER NVARCHAR(50)  = 'User'
IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationRole] AR WHERE [AR].[Name] = @ROLE_USER)
BEGIN 
    INSERT INTO [dbo].[ApplicationRole]([ApplicationRoleId],[Name],[Description]) VALUES(1, @ROLE_USER, 'Normal Users')
END
SET IDENTITY_INSERT [dbo].[ApplicationRole] OFF

SET IDENTITY_INSERT [dbo].[VideoIndexStatus] ON
DECLARE @VIDEO_INDEX_STATUS NVARCHAR(50) = 'Pending'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoIndexStatus] VIS WHERE [VIS].[Name] = @VIDEO_INDEX_STATUS)
BEGIN
    INSERT INTO [dbo].[VideoIndexStatus]([VideoIndexStatusId],[Name]) VALUES(0, @VIDEO_INDEX_STATUS)
END
SET @VIDEO_INDEX_STATUS = 'Processing'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoIndexStatus] VIS WHERE [VIS].[Name] = @VIDEO_INDEX_STATUS)
BEGIN
    INSERT INTO [dbo].[VideoIndexStatus]([VideoIndexStatusId],[Name]) VALUES(1, @VIDEO_INDEX_STATUS)
END
SET @VIDEO_INDEX_STATUS = 'Processed'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoIndexStatus] VIS WHERE [VIS].[Name] = @VIDEO_INDEX_STATUS)
BEGIN
    INSERT INTO [dbo].[VideoIndexStatus]([VideoIndexStatusId],[Name]) VALUES(2, @VIDEO_INDEX_STATUS)
END
SET IDENTITY_INSERT [dbo].[VideoIndexStatus] OFF


SET IDENTITY_INSERT [dbo].[VideoIndexingCost] ON
DECLARE @COST_PER_MINUTE MONEY = 0.21
IF NOT EXISTS (SELECT * FROM [dbo].[VideoIndexingCost] VIC WHERE [VIC].[CostPerMinute] = @COST_PER_MINUTE)
BEGIN 
    INSERT INTO [dbo].[VideoIndexingCost]([VideoIndexingCostId],[CostPerMinute]) VALUES(1, @COST_PER_MINUTE)
END
SET IDENTITY_INSERT [dbo].[VideoIndexingCost] OFF


SET IDENTITY_INSERT [dbo].[VideoIndexingMargin] ON
DECLARE @MARGIN DECIMAL(5,4) = 0.5
IF NOT EXISTS (SELECT * FROM [dbo].[VideoIndexingMargin] VIM WHERE [VIM].[Margin] = @MARGIN)
BEGIN 
    INSERT INTO [dbo].[VideoIndexingMargin]([VideoIndexingMarginId],[Margin]) VALUES(1, @MARGIN)
END
SET IDENTITY_INSERT [dbo].[VideoIndexingMargin] OFF