CREATE TABLE [dbo].[ApplicationRole]
(
	[ApplicationRoleId] SMALLINT NOT NULL CONSTRAINT PK_Application PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(250) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_ApplicationRole_Name] ON [dbo].[ApplicationRole] ([Name])