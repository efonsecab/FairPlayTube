CREATE TABLE [dbo].[VideoPlaylist]
(
	[VideoPlaylistId] BIGINT NOT NULL CONSTRAINT PK_VideoPlaylist PRIMARY KEY IDENTITY, 
    [OwnerApplicationUserId] BIGINT NOT NULL, 
    [PlaylistName] NVARCHAR(50) NOT NULL, 
    [PlaylistDescription] NVARCHAR(250) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_VideoPlaylist_ApplicationUser] FOREIGN KEY ([OwnerApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
)

GO

CREATE UNIQUE INDEX [UI_VideoPlaylist_PlaylistName] ON [dbo].[VideoPlaylist] ([OwnerApplicationUserId], [PlaylistName])
