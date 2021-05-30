CREATE TABLE [dbo].[UserExternalMonetization]
(
	[UserExternalMonetizationId] BIGINT NOT NULL CONSTRAINT PK_UserExternalMonetization PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [MonetizationUrl] NVARCHAR(1000) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_UserExternalMonetization_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_UserExternalMonetization_MonetizationUrl] ON [dbo].[UserExternalMonetization] ([ApplicationUserId], [MonetizationUrl])
