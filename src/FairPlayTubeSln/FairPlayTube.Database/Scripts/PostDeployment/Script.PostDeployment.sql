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

--START OF DEFAULT APPLICATION ROLES
SET IDENTITY_INSERT [dbo].[ApplicationRole] ON
DECLARE @ROLE_USER NVARCHAR(50)  = 'User'
IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationRole] AR WHERE [AR].[Name] = @ROLE_USER)
BEGIN 
    INSERT INTO [dbo].[ApplicationRole]([ApplicationRoleId],[Name],[Description]) VALUES(1, @ROLE_USER, 'Normal Users')
END
SET IDENTITY_INSERT [dbo].[ApplicationRole] OFF
--START OF DEFAULT APPLICATION ROLES

--START OF DEFAULT VIDEO INDEX STATUSES
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
SET @VIDEO_INDEX_STATUS = 'Deleted'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoIndexStatus] VIS WHERE [VIS].[Name] = @VIDEO_INDEX_STATUS)
BEGIN
    INSERT INTO [dbo].[VideoIndexStatus]([VideoIndexStatusId],[Name]) VALUES(3, @VIDEO_INDEX_STATUS)
END
--START OF DEFAULT VIDEO INDEX STATUSES
--START OF DEFAULT VIDEO VISIBILITY CATEGORIES
IF NOT EXISTS (SELECT * FROM [dbo].[VideoVisibility] WHERE [Name] = 'Public')
BEGIN
	INSERT INTO [dbo].VideoVisibility([VideoVisibilityId],[Name]) VALUES(1,'Public')
END
IF NOT EXISTS (SELECT * FROM [dbo].[VideoVisibility] WHERE [Name] = 'Private')
BEGIN
	INSERT INTO [dbo].VideoVisibility([VideoVisibilityId],[Name]) VALUES(2,'Private')
END
--END OF DEFAULT VIDEO VISIBILITY CATEGORIES

--IORIGINATOR INFO
DECLARE @ORIGINATORIPADDRESS NVARCHAR(100) = CONVERT(NVARCHAR(100), ISNULL(CONNECTIONPROPERTY('client_net_address'), ''))
DECLARE @SOURCEAPPLICATION NVARCHAR(250) = APP_NAME()
DECLARE @ROWCREATIONUSER NVARCHAR(256) = 'Post Deployment Script'

--START OF DEFAULT VIDEO INDEXING COSTS
SET IDENTITY_INSERT [dbo].[VideoIndexingCost] ON
DECLARE @COST_PER_MINUTE MONEY = 0.21
IF NOT EXISTS (SELECT 1 FROM [dbo].[VideoIndexingCost])
BEGIN 
    INSERT INTO [dbo].[VideoIndexingCost]([VideoIndexingCostId],[CostPerMinute],[RowCreationDateTime],[RowCreationUser],[SourceApplication],[OriginatorIPAddress]) 
    VALUES(1, @COST_PER_MINUTE, SYSUTCDATETIME(), @ROWCREATIONUSER, @SOURCEAPPLICATION, @ORIGINATORIPADDRESS)
END
SET IDENTITY_INSERT [dbo].[VideoIndexingCost] OFF
--END OF DEFAULT VIDEO INDEXING COSTS

--START OF DEFAULT VIDEO INDEXING MARGINS
SET IDENTITY_INSERT [dbo].[VideoIndexingMargin] ON
DECLARE @MARGIN DECIMAL(5,4) = 0.5
IF NOT EXISTS (SELECT 1 FROM [dbo].[VideoIndexingMargin])
BEGIN 
    INSERT INTO [dbo].[VideoIndexingMargin]([VideoIndexingMarginId],[Margin],[RowCreationDateTime],[RowCreationUser],[SourceApplication],[OriginatorIPAddress]) 
    VALUES(1, @MARGIN, SYSUTCDATETIME(), @ROWCREATIONUSER, @SOURCEAPPLICATION, @ORIGINATORIPADDRESS)
END
SET IDENTITY_INSERT [dbo].[VideoIndexingMargin] OFF
--END OF DEFAULT VIDEO INDEXING MARGINS