CREATE TABLE [dbo].[VideoIndexingTransaction]
(
	[VideoIndexingTransactionId] BIGINT NOT NULL CONSTRAINT PK_VideoIndexingTransaction PRIMARY KEY IDENTITY, 
    [VideoInfoId] BIGINT NOT NULL,
    [IndexingCost] MONEY NOT NULL DEFAULT 0,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_VideoIndexingTransaction_VideoInfo] FOREIGN KEY ([VideoInfoId]) REFERENCES [VideoInfo]([VideoInfoId]), 
)
