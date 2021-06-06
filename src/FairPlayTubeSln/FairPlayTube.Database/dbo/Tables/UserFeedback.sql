CREATE TABLE [dbo].[UserFeedback]
(
	[UserFeedbackId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_UserFeedback PRIMARY KEY,
	[ShortDescription] NVARCHAR(500) NOT NULL,
	[DetailedDescription] NVARCHAR(MAX) NOT NULL,
	[ScreenshotUrl] NVARCHAR(1000) NOT NULL,
	[ApplicationUserId] BIGINT NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
	CONSTRAINT FK_UserFeedback_ApplicationUserId FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId])
)

GO