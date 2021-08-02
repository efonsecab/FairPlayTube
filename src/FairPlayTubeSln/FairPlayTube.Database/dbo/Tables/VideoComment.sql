CREATE TABLE [dbo].[VideoComment]
(
	[VideoCommentId] BIGINT NOT NULL CONSTRAINT PK_VideoComment PRIMARY KEY,
	[VideoInfoId] BIGINT NOT NULL CONSTRAINT FK_VideoInfo_VideoComment REFERENCES [dbo].[VideoInfo]([VideoInfoId]),
	[ApplicationUserId] BIGINT NOT NULL CONSTRAINT FK_ApplicationUserId_VideoComment REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	[Comment] NVARCHAR(500) NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256),
    [SourceApplication] NVARCHAR(250), 
    [OriginatorIPAddress] NVARCHAR(100),
)
