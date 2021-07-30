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
--START OF DEFAULT VIDEO INDEX STATUSES
--START OF DEFAULT VIDEO VISIBILITY CATEGORIES
SET IDENTITY_INSERT [dbo].[VideoVisibility] ON
INSERT INTO [dbo].VideoVisibility([VideoVisibilityId],[Name]) VALUES(1,'Public')
INSERT INTO [dbo].VideoVisibility([VideoVisibilityId],[Name]) VALUES(2,'Private')
SET IDENTITY_INSERT [dbo].[VideoVisibility] OFF
--END OF DEFAULT VIDEO VISIBILITY CATEGORIES