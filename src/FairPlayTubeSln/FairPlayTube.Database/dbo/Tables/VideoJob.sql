CREATE TABLE [dbo].[VideoJob]
(
	[VideoJobId] BIGINT NOT NULL CONSTRAINT PK_VideoJob PRIMARY KEY IDENTITY, 
    [VideoInfoId] BIGINT NOT NULL, 
    [Budget] MONEY NOT NULL, 
    [Title] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(250) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_VideoJob_VideoInfo] FOREIGN KEY ([VideoInfoId]) REFERENCES [VideoInfo]([VideoInfoId]), 
)
