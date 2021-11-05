CREATE TABLE [dbo].[VideoJobEscrow]
(
	[VideoJobEscrowId] BIGINT NOT NULL CONSTRAINT PK_VideoJobEscrow PRIMARY KEY IDENTITY, 
    [VideoJobId] BIGINT NOT NULL, 
    [Amount] MONEY NOT NULL, 
    [PaypalPayoutBatchItemId] BIGINT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
    CONSTRAINT [FK_VideoJobEscrow_PaypalPayoutBatchItem] FOREIGN KEY ([PaypalPayoutBatchItemId]) REFERENCES [PaypalPayoutBatchItem]([PaypalPayoutBatchItemId]), 
    CONSTRAINT [FK_VideoJobEscrow_VideoJob] FOREIGN KEY ([VideoJobId]) REFERENCES [VideoJob]([VideoJobId])
)

GO

CREATE UNIQUE INDEX [UI_VideoJobEscrow_VideoJobId] ON [dbo].[VideoJobEscrow] ([VideoJobId])
