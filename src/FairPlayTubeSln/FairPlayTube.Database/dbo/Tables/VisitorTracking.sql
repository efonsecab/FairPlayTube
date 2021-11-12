CREATE TABLE [dbo].[VisitorTracking]
(
	[VisitorTrackingId] BIGINT NOT NULL CONSTRAINT PK_VisitorTracking PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NULL, 
 [RemoteIpAddress] NVARCHAR(250) NOT NULL, 
    [Country] NVARCHAR(250) NOT NULL, 
    [VisitDateTime] DATETIMEOFFSET NOT NULL, 
    [UserAgent] NVARCHAR(MAX) NOT NULL, 
    [Host] NVARCHAR(MAX) NOT NULL, 
    [VisitedUrl] NVARCHAR(MAX) NOT NULL
    CONSTRAINT [FK_VisitorTracking_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
    [VideoInfoId] BIGINT NULL, 
    [SessionId] UNIQUEIDENTIFIER NULL, 
    [LastTrackedDateTime] DATETIMEOFFSET NULL, 
    CONSTRAINT [FK_VisitorTracking_VideoInfo] FOREIGN KEY ([VideoInfoId]) REFERENCES [VideoInfo]([VideoInfoId])
)

GO

CREATE INDEX [IX_VisitorTracking_VideoId] ON [dbo].[VisitorTracking] ([VideoInfoId])
