CREATE TABLE [dbo].[ApplicationUserApiRequest]
(
	[ApplicationUserApiRequestId] BIGINT NOT NULL CONSTRAINT PK_ApplicationUserApiRequest PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [Method] NVARCHAR(MAX) NOT NULL, 
    [Path] NVARCHAR(MAX) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
    CONSTRAINT [FK_ApplicationUserApiRequest_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)
