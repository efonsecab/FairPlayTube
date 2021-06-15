CREATE TABLE [dbo].[UserVideoRating]
(
	[UserVideoRatingId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_UserVideoRating PRIMARY KEY,
	[ApplicationUserId] BIGINT NOT NULL,
	[VideoInfoId] BIGINT NOT NULL,
	[Rating] TINYINT NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
	CONSTRAINT FK_UserVideoRating_ApplicationUserId FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	CONSTRAINT FK_UserVideoRating_VideoInfoId FOREIGN KEY ([VideoInfoId]) REFERENCES [dbo].[VideoInfo]([VideoInfoId])
)

GO

CREATE UNIQUE INDEX [UI_[UserVideoRating_ApplicationUserId] ON [dbo].[UserVideoRating] ([ApplicationUserId])

GO

CREATE UNIQUE INDEX [UI_[UserVideoRating_VideoInfoId] ON [dbo].[UserVideoRating] ([VideoInfoId])