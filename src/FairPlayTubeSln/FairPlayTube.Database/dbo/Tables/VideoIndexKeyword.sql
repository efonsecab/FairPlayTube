CREATE TABLE [dbo].[VideoIndexKeyword]
(
	[VideoIndexKwywordId] BIGINT NOT NULL CONSTRAINT PK_VideoIndex PRIMARY KEY IDENTITY, 
    [VideoInfoId] BIGINT NOT NULL, 
    [Keyword] NVARCHAR(500) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_VideoIndexKeyword_VideoInfo] FOREIGN KEY ([VideoInfoId]) REFERENCES [VideoInfo]([VideoInfoId])
)

GO

CREATE UNIQUE INDEX [UI_VideoIndexKeyword_Keyword] ON [dbo].[VideoIndexKeyword] ([VideoInfoId], [Keyword])
