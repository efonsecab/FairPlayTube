CREATE TABLE [dbo].[VideoJobApplication]
(
	[VideoJobApplicationId] BIGINT NOT NULL CONSTRAINT PK_VideoJobApplication PRIMARY KEY,
	[VideoJobId] BIGINT NOT NULL CONSTRAINT FK_VideoJobApplication_VideoJobId REFERENCES [dbo].[VideoJob]([VideoJobId]),
	[ApplicantApplicationUserId] BIGINT NOT NULL CONSTRAINT FK_VideoJobApplication_ApplicationUser REFERENCES [dbo].[ApplicationUser]([ApplicationUserId]),
	[ApplicantCoverLetter] TEXT NOT NULL,
	[VideoJobApplicationStatusId] SMALLINT NOT NULL, 
	[RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_VideoJobApplication_VideoJobApplicationStatus] FOREIGN KEY ([VideoJobApplicationStatusId]) REFERENCES [VideoJobApplicationStatus]([VideoJobApplicationStatusId]), 
)

GO
