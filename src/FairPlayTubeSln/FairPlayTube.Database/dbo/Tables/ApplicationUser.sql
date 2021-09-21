CREATE TABLE [dbo].[ApplicationUser] (
    [ApplicationUserId] BIGINT NOT NULL IDENTITY CONSTRAINT PK_ApplicationUser PRIMARY KEY, 
    [FullName] NVARCHAR(150) NOT NULL, 
    [EmailAddress] NVARCHAR(150) NOT NULL, 
    [LastLogIn] DATETIMEOFFSET NOT NULL, 
    [AzureAdB2CObjectId] UNIQUEIDENTIFIER NOT NULL ,
    [AvailableFunds] MONEY NOT NULL DEFAULT 0, 
    [ApplicationUserStatusId] SMALLINT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_ApplicationUser_ApplicationUserStatus] FOREIGN KEY ([ApplicationUserStatusId]) REFERENCES [ApplicationUserStatus]([ApplicationUserStatusId]),
);


GO

CREATE UNIQUE INDEX [UI_ApplicationUser_AzureAdB2CObjectId] ON [dbo].[ApplicationUser] ([AzureAdB2CObjectId])