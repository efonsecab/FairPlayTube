CREATE TABLE [dbo].[PaypalPayoutBatchItem]
(
	[PaypalPayoutBatchItemId] BIGINT NOT NULL CONSTRAINT PK_PaypalPayoutBatchItem PRIMARY KEY IDENTITY, 
    [PaypalPayoutBatchId] BIGINT NOT NULL, 
    [RecipientType] NVARCHAR(50) NOT NULL, 
    [AmountValue] MONEY NOT NULL, 
    [AmountCurrency] NVARCHAR(50) NOT NULL, 
    [Note] TEXT NOT NULL, 
    [SenderItemId] UNIQUEIDENTIFIER NOT NULL, 
    [ReceiverEmailAddress] NVARCHAR(150) NOT NULL, 
    [NotificationLanguage] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_PaypalPayoutBatchItem_PaypalPayoutBatch] FOREIGN KEY ([PaypalPayoutBatchId]) REFERENCES [PaypalPayoutBatch]([PaypalPayoutBatchId])
)
