CREATE TABLE [dbo].[VideoJobApplicationStatus]
(
	[VideoJobApplicationStatusId] SMALLINT CONSTRAINT PK_VideoJobApplicationStatus NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(2150) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_VideoJobApplicationStatus_Name] ON [dbo].[VideoJobApplicationStatus] ([Name])
