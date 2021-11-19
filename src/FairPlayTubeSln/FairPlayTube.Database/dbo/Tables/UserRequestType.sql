CREATE TABLE [dbo].[UserRequestType]
(
	[UserRequestTypeId] SMALLINT NOT NULL CONSTRAINT PK_UserRequestType PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_UserRequestType_Name] ON [dbo].[UserRequestType] ([Name])
