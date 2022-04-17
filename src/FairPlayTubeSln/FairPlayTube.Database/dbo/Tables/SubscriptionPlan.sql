CREATE TABLE [dbo].[SubscriptionPlan]
(
	[SubscriptionPlanId] SMALLINT NOT NULL CONSTRAINT PK_SubscriptionPlan PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(250) NOT NULL, 
    [MaxAllowedWeeklyVideos] SMALLINT NULL
)

GO

CREATE UNIQUE INDEX [UI_SubscriptionPlan_Name] ON [dbo].[SubscriptionPlan] ([Name])
