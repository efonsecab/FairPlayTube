CREATE TABLE [dbo].[RoleRequest]
(
	[RoleRequestId] BIGINT NOT NULL CONSTRAINT PK_RoleRequest PRIMARY KEY IDENTITY,
	[ApplicationUserId] BIGINT NOT NULL,
	[RequestedApplicationRoleId] BIGINT NOT NULL,
	[Name] NVARCHAR(150) NOT NULL,
	[Lastname] NVARCHAR(150) NOT NULL,
	[Surname] NVARCHAR(150) NOT NULL,
	[Nationality] NVARCHAR(100) NOT NULL,
	[NationalId] INT NOT NULL,
	[BirthDate] DATETIME NOT NULL,
	[RequestReason] NVARCHAR(200) NOT NULL,
	[IsApproved] BIT NOT NULL,
	CONSTRAINT [FK_RoleRequest_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId])
)
