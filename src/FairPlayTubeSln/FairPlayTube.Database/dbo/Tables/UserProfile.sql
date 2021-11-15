CREATE TABLE [dbo].[UserProfile]
(
	[UserProfileId] BIGINT NOT NULL CONSTRAINT PK_UserProfile PRIMARY KEY IDENTITY,
	[ApplicationUserId] BIGINT NOT NULL CONSTRAINT FK_ApplicationUserId_UserProfile FOREIGN KEY REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	[UserVerificationStatusId] BIGINT NOT NULL CONSTRAINT FK_UserProfile_VerificationStatus FOREIGN KEY REFERENCES [dbo].[UserVerificationStatus]([UserVerificationStatusId]),
	[About] NVARCHAR(100) NOT NULL,
	[Topics] NVARCHAR(100) NOT NULL,
	[Nationality] NVARCHAR(20) NOT NULL,
	[NationalIdNumber] INT NOT NULL,
	[NationalIdPhotoUrl] NVARCHAR(200) NOT NULL,
	[DisplayAlias] NVARCHAR(100), 
    [PaypalEmailAddress] NVARCHAR(500) NULL
)

GO

CREATE UNIQUE INDEX [UI_UserProfile_PaypalEmailAddress] ON [dbo].[UserProfile] ([PaypalEmailAddress])
