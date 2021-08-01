CREATE TABLE [dbo].[ApplicationUserFeature]
(
	[ApplicationUserFeatureId] BIGINT NOT NULL CONSTRAINT PK_ApplicationUserFeature PRIMARY KEY IDENTITY,
    [ApplicationUserId] BIGINT NOT NULL,
    [GatedFeatureId] INT NOT NULL, 
    [Enabled] BIT NOT NULL, 
    CONSTRAINT [FK_ApplicationUserFeature_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]), 
    CONSTRAINT [FK_ApplicationUserFeature_GatedFeature] FOREIGN KEY ([GatedFeatureId]) REFERENCES [GatedFeature]([GatedFeatureId])
)

GO

CREATE UNIQUE INDEX [UI_ApplicationUserFeature_] ON [dbo].[ApplicationUserFeature] ([ApplicationUserId], [GatedFeatureId])
