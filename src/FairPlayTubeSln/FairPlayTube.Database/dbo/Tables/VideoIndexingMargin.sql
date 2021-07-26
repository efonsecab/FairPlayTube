CREATE TABLE [dbo].[VideoIndexingMargin]
(
    [VideoIndexingMarginId] BIGINT NOT NULL CONSTRAINT PK_VideoIndexingMargin PRIMARY KEY IDENTITY, 
    [Margin] DECIMAL(5,4) NOT NULL,

    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256),
    [SourceApplication] NVARCHAR(250), 
    [OriginatorIPAddress] NVARCHAR(100),

    CONSTRAINT BETWEEN_0_AND_1 CHECK ( 0 <= [Margin] AND [Margin] <= 1)
)
