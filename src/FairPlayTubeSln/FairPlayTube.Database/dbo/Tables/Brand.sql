CREATE TABLE [dbo].[Brand]
(
	[BrandId] BIGINT NOT NULL CONSTRAINT PK_Brand PRIMARY KEY,
	[Name] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(500) NOT NULL,
	[ApplicationUserId] BIGINT NOT NULL,
	CONSTRAINT FK_Brand_ApplicationUserId FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[ApplicationUser]([ApplicationUserId])
)

GO

CREATE UNIQUE INDEX [UI_Brand_Name] ON [dbo].[Brand] ([Name])
