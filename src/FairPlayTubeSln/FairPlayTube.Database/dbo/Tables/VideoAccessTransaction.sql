CREATE TABLE [dbo].[VideoAccessTransaction]
(
	[VideoAccessTransactionId] BIGINT NOT NULL CONSTRAINT PK_VideoAccessTransaction PRIMARY KEY, 
    [VideoInfoId] BIGINT NOT NULL, 
    [BuyerApplicationUserId] BIGINT NOT NULL, 
    [AppliedPrice] MONEY NOT NULL, 
    [AppliedCommission] MONEY NOT NULL, 
    [TotalPrice] MONEY NOT NULL, 
    CONSTRAINT [FK_VideoAccessTransaction_VideoInfo] FOREIGN KEY ([VideoInfoId]) REFERENCES [VideoInfo]([VideoInfoId]), 
    CONSTRAINT [FK_VideoAccessTransaction_ApplicationUser] FOREIGN KEY ([BuyerApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)
GO

CREATE UNIQUE INDEX [UI_VideoAccessTransaction_BuyerApplicationUserIdVideoInfoId] ON [dbo].[VideoAccessTransaction] ([VideoInfoId], [BuyerApplicationUserId])
