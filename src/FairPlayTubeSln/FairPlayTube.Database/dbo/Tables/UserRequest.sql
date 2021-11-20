CREATE TABLE [dbo].[UserRequest]
(
	[UserRequestId] BIGINT NOT NULL CONSTRAINT PK_UserRequest PRIMARY KEY IDENTITY, 
    [UserRequestTypeId] SMALLINT NOT NULL, 
    [Description] NVARCHAR(1000) NOT NULL,
    [EmailAddress] NVARCHAR(150) NOT NULL,
    [ApplicationUserId] BIGINT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
    CONSTRAINT [FK_UserRequest_UserRequestType] FOREIGN KEY ([UserRequestTypeId]) REFERENCES [UserRequestType]([UserRequestTypeId]), 
    CONSTRAINT [FK_UserRequest_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
)
