CREATE TABLE [dbo].[Resource]
(
	[ResourceId] INT NOT NULL CONSTRAINT PK_Resource PRIMARY KEY IDENTITY, 
    [Type] NVARCHAR(1500) NOT NULL, 
    [Key] NVARCHAR(50) NOT NULL, 
    [Value] TEXT NOT NULL, 
    [CultureId] INT NOT NULL, 
    CONSTRAINT [FK_Resource_Culture] FOREIGN KEY ([CultureId]) REFERENCES [Culture]([CultureId])
)

GO

CREATE UNIQUE INDEX [UI_Resource_Type_Key_CultureId] ON [dbo].[Resource] ([Type],[Key], [CultureId])
