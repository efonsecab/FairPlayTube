CREATE TABLE [dbo].[UserVerificationStatus]
(
	[UserVerificationStatusId] BIGINT CONSTRAINT PK_UserVerificationStatus PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(200) NOT NULL
)
