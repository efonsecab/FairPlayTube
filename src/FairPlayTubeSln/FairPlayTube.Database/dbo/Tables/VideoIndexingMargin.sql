CREATE TABLE [dbo].[VideoIndexingMargin]
(
    [VideoIndexingMarginId] BIGINT NOT NULL CONSTRAINT PK_VideoIndexingMargin PRIMARY KEY IDENTITY, 
    [Margin] TINYINT NOT NULL,

    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,

    CONSTRAINT BETWEEN_1_AND_100 CHECK ( 1 <= [Margin] AND [Margin] <= 100)
)
