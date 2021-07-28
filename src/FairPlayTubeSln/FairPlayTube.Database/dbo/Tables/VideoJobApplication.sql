CREATE TABLE [dbo].[VideoJobApplication]
(
	[VideoJobApplicationId] BIGINT NOT NULL CONSTRAINT PK_VideoJobApplication PRIMARY KEY,
	[VideoInfoId] BIGINT NOT NULL CONSTRAINT FK_VideoJobApplication_VideoInfoId REFERENCES [dbo].[VideoInfo]([VideoInfoId]),
	[ApplicantApplicationUserId] BIGINT NOT NULL CONSTRAINT FK_VideoJobApplication_ApplicationUser REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	[ApplicantCoverLetter] TEXT NOT NULL,
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
)
