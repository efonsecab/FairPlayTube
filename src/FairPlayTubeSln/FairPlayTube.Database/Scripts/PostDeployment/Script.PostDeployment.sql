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
--START OF DEFAULT APPLICATION USER STATUS
DECLARE @APPLICATION_USER_STATUS_NAME NVARCHAR(50) = 'PendingApproval'
IF NOT EXISTS(SELECT * FROM [dbo].[ApplicationUserStatus] AUS WHERE [AUS].[Name] = @APPLICATION_USER_STATUS_NAME)
BEGIN
    INSERT INTO [dbo].[ApplicationUserStatus]([ApplicationUserStatusId],[Name],[Description]) 
    VALUES(1, @APPLICATION_USER_STATUS_NAME, 'User has pending approval')
END
SET @APPLICATION_USER_STATUS_NAME = 'Approved'
IF NOT EXISTS(SELECT * FROM [dbo].[ApplicationUserStatus] AUS WHERE [AUS].[Name] = @APPLICATION_USER_STATUS_NAME)
BEGIN
    INSERT INTO [dbo].[ApplicationUserStatus]([ApplicationUserStatusId],[Name],[Description]) 
    VALUES(2, @APPLICATION_USER_STATUS_NAME, 'User has received approval')
END
--END OF DEFAULT APPLICATION USER STATUS
--START OF DEFAULT CULTURES
DECLARE @CULTURE NVARCHAR(50) = 'en-US'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(1, @CULTURE)
END
SET @CULTURE='es-CR'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(2, @CULTURE)
END
SET @CULTURE='fr-CA'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(3, @CULTURE)
END
SET @CULTURE='it'
IF NOT EXISTS (SELECT * FROM [dbo].[Culture] WHERE [Name] = @CULTURE)
BEGIN
    INSERT INTO Culture([CultureId],[Name]) VALUES(4, @CULTURE)
END
--END OF DEFAULT CULTURES
--START OF VIDEO JOB APPLICATION STATUS
DECLARE @VIDEOJOBAPPLICATIONSTATUSNAME NVARCHAR(50) = 'New'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoJobApplicationStatus] WHERE [Name] = @VIDEOJOBAPPLICATIONSTATUSNAME)
BEGIN
    INSERT INTO VideoJobApplicationStatus([VideoJobApplicationStatusId],[Name],[Description]) VALUES(1, @VIDEOJOBAPPLICATIONSTATUSNAME, 'Application is new')
END
SET @VIDEOJOBAPPLICATIONSTATUSNAME = 'Selected'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoJobApplicationStatus] WHERE [Name] = @VIDEOJOBAPPLICATIONSTATUSNAME)
BEGIN
    INSERT INTO VideoJobApplicationStatus([VideoJobApplicationStatusId],[Name], [Description]) VALUES(2, @VIDEOJOBAPPLICATIONSTATUSNAME, 'Application has been selected')
END
SET @VIDEOJOBAPPLICATIONSTATUSNAME = 'Not Selected'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoJobApplicationStatus] WHERE [Name] = @VIDEOJOBAPPLICATIONSTATUSNAME)
BEGIN
    INSERT INTO VideoJobApplicationStatus([VideoJobApplicationStatusId],[Name],[Description]) VALUES(3, @VIDEOJOBAPPLICATIONSTATUSNAME, 'Application has not been selected')
END
SET @VIDEOJOBAPPLICATIONSTATUSNAME = 'Pending Payment'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoJobApplicationStatus] WHERE [Name] = @VIDEOJOBAPPLICATIONSTATUSNAME)
BEGIN
    INSERT INTO VideoJobApplicationStatus([VideoJobApplicationStatusId],[Name],[Description]) VALUES(4, @VIDEOJOBAPPLICATIONSTATUSNAME, 'Work has been performed. Payment is pending')
END
SET @VIDEOJOBAPPLICATIONSTATUSNAME = 'Paid'
IF NOT EXISTS (SELECT * FROM [dbo].[VideoJobApplicationStatus] WHERE [Name] = @VIDEOJOBAPPLICATIONSTATUSNAME)
BEGIN
    INSERT INTO VideoJobApplicationStatus([VideoJobApplicationStatusId],[Name],[Description]) VALUES(5, @VIDEOJOBAPPLICATIONSTATUSNAME, 'Work has been performed and it is already paid')
END
--END OF VIDEO JOB APPLICATION STATUS
