CREATE TABLE [dbo].[GatedFeature]
(
	[GatedFeatureId] INT NOT NULL CONSTRAINT PK_GatedFeature PRIMARY KEY IDENTITY, 
    [FeatureName] NVARCHAR(250) NOT NULL, 
    [DefaultValue] BIT NOT NULL DEFAULT 1
)

GO

CREATE UNIQUE INDEX [UI_GatedFeature_FeatureName] ON [dbo].[GatedFeature] ([FeatureName])
