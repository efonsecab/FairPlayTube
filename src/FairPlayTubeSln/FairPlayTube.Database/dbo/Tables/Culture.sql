CREATE TABLE [dbo].[Culture]
(
	[CultureId] INT NOT NULL CONSTRAINT PK_Culture PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_Culture_Name] ON [dbo].[Culture] ([Name])
