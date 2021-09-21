CREATE TABLE [dbo].[UserInvitation]
(
	[UserInvitationId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_UserInvitation PRIMARY KEY,
	[InvitingApplicationUserId] BIGINT NOT NULL,
    [InvitedUserEmail] NVARCHAR(150) NOT NULL,
	[InviteCode] UNIQUEIDENTIFIER NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
	CONSTRAINT FK_UserInvitation_InvitingApplicationUserId FOREIGN KEY ([InvitingApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_UserInvitation_InvitedUserEmail] ON [dbo].[UserInvitation] ([InvitedUserEmail])

GO

CREATE UNIQUE INDEX [UI_UserInvitation_InviteCode] ON [dbo].[UserInvitation] ([InviteCode])
