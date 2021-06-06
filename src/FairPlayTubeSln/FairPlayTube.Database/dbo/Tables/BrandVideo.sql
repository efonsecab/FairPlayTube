CREATE TABLE [dbo].[BrandVideo]
(
	[BrandVideoId] BIGINT NOT NULL CONSTRAINT PK_BrandVideo PRIMARY KEY, 
    [BrandId] BIGINT NOT NULL, 
    [VideoInfoId] BIGINT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    CONSTRAINT FK_BrandVideo_Brand FOREIGN KEY ([BrandId]) REFERENCES [dbo].[Brand]([BrandId]),
    CONSTRAINT FK_BrandVideo_VideoInfo FOREIGN KEY ([VideoInfoId]) REFERENCES [dbo].[VideoInfo]([VideoInfoId])
)

GO