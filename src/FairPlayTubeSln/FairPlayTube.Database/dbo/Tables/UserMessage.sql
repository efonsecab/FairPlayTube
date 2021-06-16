CREATE TABLE [dbo].[UserMessage]
(
	[UserMessageId] BIGINT NOT NULL CONSTRAINT PK_UserMessage PRIMARY KEY IDENTITY,
	[FromApplicationUserId] BIGINT NOT NULL CONSTRAINT FK_FromApplicationUserId_ApplicationUser FOREIGN KEY REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	[ToApplicationUserId] BIGINT NOT NULL CONSTRAINT FK_ToApplicationUserId_ApplicationUser FOREIGN KEY REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	[Message] NVARCHAR(MAX) NOT NULL
)
