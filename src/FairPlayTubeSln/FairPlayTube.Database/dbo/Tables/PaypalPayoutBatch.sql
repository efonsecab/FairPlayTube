CREATE TABLE [dbo].[PaypalPayoutBatch]
(
	[PaypalPayoutBatchId] BIGINT NOT NULL CONSTRAINT PK_PaypalPayout PRIMARY KEY IDENTITY, 
    [PayoutBatchId] NVARCHAR(50) NOT NULL,
    [SenderBatchId] NVARCHAR(50) NOT NULL, 
    [EmailSubject] NVARCHAR(250) NOT NULL, 
    [EmailMessage] TEXT NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_PaypalPayoutBatch_SenderBatchId] ON [dbo].[PaypalPayoutBatch] ([SenderBatchId])

GO

CREATE UNIQUE INDEX [UI_PaypalPayoutBatch_PayoutBatchId] ON [dbo].[PaypalPayoutBatch] ([PayoutBatchId])
