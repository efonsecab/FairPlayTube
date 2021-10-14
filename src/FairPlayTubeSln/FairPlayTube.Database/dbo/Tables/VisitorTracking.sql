CREATE TABLE [dbo].[VisitorTracking]
(
	[VisitorTrackingId] BIGINT NOT NULL CONSTRAINT PK_VisitorTracking PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NULL, 
 [RemoteIpAddress] NVARCHAR(250) NOT NULL, 
    [Country] NVARCHAR(250) NOT NULL, 
    [VisitDateTime] DATETIMEOFFSET NOT NULL, 
    [UserAgent] NVARCHAR(250) NOT NULL, 
    [Host] NVARCHAR(250) NOT NULL, 
    [VisitedUrl] NVARCHAR(250) NOT NULL
    CONSTRAINT [FK_VisitorTracking_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
    [VideoId] NVARCHAR(50) NULL
)

GO

CREATE INDEX [IX_VisitorTracking_VideoId] ON [dbo].[VisitorTracking] ([VideoId])
