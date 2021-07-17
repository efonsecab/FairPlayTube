CREATE TABLE [dbo].[VideoIndexingCost]
(
    [VideoIndexingCostId] BIGINT NOT NULL CONSTRAINT PK_VideoIndexingCost PRIMARY KEY IDENTITY, 
    [CostPerMinute] MONEY NOT NULL DEFAULT 0,
    
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
)
