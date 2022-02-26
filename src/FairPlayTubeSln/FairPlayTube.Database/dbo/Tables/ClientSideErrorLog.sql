CREATE TABLE [dbo].[ClientSideErrorLog]
(
	[ClientSideErrorLogId] BIGINT NOT NULL CONSTRAINT PK_ClientSideErrorLog PRIMARY KEY IDENTITY, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [StackTrace] NVARCHAR(MAX) NOT NULL, 
    [FullException] NVARCHAR(MAX) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
)