CREATE TABLE [dbo].[ApplicationUserStatus]
(
	[ApplicationUserStatusId] SMALLINT NOT NULL CONSTRAINT PK_ApplicationUserStatus PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(250) NOT NULL
)

GO

CREATE INDEX [UI_ApplicationUserStatus_ApplicationUserStatusName] ON [dbo].[ApplicationUserStatus] ([Name])
