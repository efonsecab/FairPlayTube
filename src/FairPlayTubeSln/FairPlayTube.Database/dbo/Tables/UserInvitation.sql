CREATE TABLE [dbo].[UserInvitation]
(
	[UserInvitationId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_UserInvitation PRIMARY KEY,
	[InvitingApplicationUserId] BIGINT NOT NULL,
    [InvitedUserEmail] NVARCHAR(150) NOT NULL, 
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
	CONSTRAINT FK_UserInvitation_InvitingApplicationUserId FOREIGN KEY ([InvitingApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId])
)

GO