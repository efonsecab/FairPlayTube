CREATE TABLE [dbo].[UserFollower]
(
	[UserFollowerId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_UserFollower PRIMARY KEY,
	[FollowedApplicationUserId] BIGINT NOT NULL,
	[FollowerApplicationUserId] BIGINT NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
	CONSTRAINT FK_UserFollower_FollowedApplicationUserId FOREIGN KEY ([FollowedApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	CONSTRAINT FK_UserFollower_FollowerApplicationUserId FOREIGN KEY ([FollowerApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId])
)

GO