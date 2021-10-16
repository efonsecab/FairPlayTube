CREATE TABLE [dbo].[UserYouTubeChannel]
(
	[UserYouTubeChannelId] BIGINT NOT NULL CONSTRAINT PK_UserYouTubeChannel PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [YouTubeChannelId] NVARCHAR(50) NOT NULL,
	[SourceApplication] NVARCHAR(250) NOT NULL,
	[OriginatorIpaddress] NVARCHAR(100) NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL,
	[RowCreationUser] NVARCHAR(256) NOT NULL
    CONSTRAINT [FK_UserYouTubeChannel_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_UserYouTubeChannel_UserYouTubeChannel] ON [dbo].[UserYouTubeChannel] ([ApplicationUserId], [YouTubeChannelId])
