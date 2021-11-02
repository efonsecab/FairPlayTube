CREATE TABLE [dbo].[VideoPlaylistItem]
(
	[VideoPlaylistItemId] BIGINT NOT NULL CONSTRAINT PK_VideoPlaylistItem PRIMARY KEY IDENTITY, 
    [VideoPlaylistId] BIGINT NOT NULL, 
    [VideoInfoId] BIGINT NULL, 
    [Order] INT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_VideoPlaylistItem_VideoPlaylist] FOREIGN KEY ([VideoPlaylistId]) REFERENCES [VideoPlaylist]([VideoPlaylistId]), 
    CONSTRAINT [FK_VideoPlaylistItem_VideoInfo] FOREIGN KEY ([VideoInfoId]) REFERENCES [VideoInfo]([VideoInfoId])
)

GO


CREATE UNIQUE INDEX [UI_VideoPlaylistItem_Video] ON [dbo].[VideoPlaylistItem] ([VideoPlaylistId], [VideoInfoId])

GO

CREATE UNIQUE INDEX [UI_VideoPlaylistItem_VideoOrder] ON [dbo].[VideoPlaylistItem] ([VideoInfoId], [Order])
