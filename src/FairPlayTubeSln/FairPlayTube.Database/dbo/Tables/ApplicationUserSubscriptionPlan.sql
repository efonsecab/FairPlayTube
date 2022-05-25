CREATE TABLE [dbo].[ApplicationUserSubscriptionPlan]
(
	[ApplicationUserSubscriptionPlan] BIGINT NOT NULL CONSTRAINT PK_ApplicationUserSubscriptionPlan PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [SubscriptionPlanId] SMALLINT NOT NULL, 
    CONSTRAINT [FK_ApplicationUserSubscriptionPlan_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
    CONSTRAINT [FK_ApplicationUserSubscriptionPlan_SubscriptionPlan] FOREIGN KEY ([SubscriptionPlanId]) REFERENCES [SubscriptionPlan]([SubscriptionPlanId])
)

GO

CREATE UNIQUE INDEX [UI_ApplicationUserSubscriptionPlan] ON [dbo].[ApplicationUserSubscriptionPlan] ([ApplicationUserId])
