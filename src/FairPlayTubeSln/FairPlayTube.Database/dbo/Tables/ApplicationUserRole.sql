CREATE TABLE [dbo].[ApplicationUserRole]
(
	[ApplicationUserRoleId] BIGINT NOT NULL CONSTRAINT PK_ApplicationUserRole PRIMARY KEY IDENTITY, 
    [ApplicationUserId] BIGINT NOT NULL, 
    [ApplicationRoleId] SMALLINT NOT NULL, 
    CONSTRAINT [FK_ApplicationUserRole_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [ApplicationUser]([ApplicationUserId]),
    CONSTRAINT [FK_ApplicationUserRole_ApplicationRole] FOREIGN KEY ([ApplicationRoleId]) REFERENCES [ApplicationRole]([ApplicationRoleId])
)

GO

CREATE UNIQUE INDEX [UI_ApplicationUserRole_ApplicationUserId] ON [dbo].[ApplicationUserRole] ([ApplicationUserId])