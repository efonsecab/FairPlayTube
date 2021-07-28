CREATE TABLE [dbo].[Person]
(
	[PersonId] BIGINT NOT NULL CONSTRAINT PK_Person PRIMARY KEY IDENTITY,
    [Id] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, 
    [SampleFaceId] NVARCHAR(50) NOT NULL, 
    [SampleFaceSourceType] NVARCHAR(50) NOT NULL, 
    [SampleFaceState] NVARCHAR(50) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [RowCreationUser] NVARCHAR(256) NOT NULL,
    [SourceApplication] NVARCHAR(250) NOT NULL, 
    [OriginatorIPAddress] NVARCHAR(100) NOT NULL, 
    [SampleFaceUrl] NVARCHAR(500) NOT NULL
)
