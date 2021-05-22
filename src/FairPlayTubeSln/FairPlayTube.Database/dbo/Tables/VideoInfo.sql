CREATE TABLE [dbo].[VideoInfo]
(
	[VideoInfoId] BIGINT NOT NULL CONSTRAINT PK_VideoInfo PRIMARY KEY IDENTITY, 
    [AccountId] UNIQUEIDENTIFIER NOT NULL, 
    [VideoId] NVARCHAR(50) NOT NULL, 
    [Location] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(500) NULL, 
    [FileName] NVARCHAR(50) NOT NULL, 
    [VideoBloblUrl] NVARCHAR(500) NOT NULL,
    [IndexedVideoUrl] NVARCHAR(500) NOT NULL,
    [ApplicationUserId] BIGINT NOT NULL, 
    [VideoIndexStatusId] SMALLINT NOT NULL, 
    CONSTRAINT [FK_VideoInfo_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
    CONSTRAINT [FK_VideoInfo_VideoIndexStatus] FOREIGN KEY ([VideoIndexStatusId]) REFERENCES [VideoIndexStatus]([VideoIndexStatusId])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Video Owner Id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'VideoInfo',
    @level2type = N'COLUMN',
    @level2name = N'ApplicationUserId'