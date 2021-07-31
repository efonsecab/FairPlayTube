CREATE TABLE [dbo].[VideoCommentAnalysis]
(
	[VideoCommentAnalysis] BIGINT NOT NULL CONSTRAINT PK_VideoCommentAnalysis PRIMARY KEY IDENTITY, 
    [VideoCommentId] BIGINT NOT NULL, 
    [Sentiment] NVARCHAR(50) NOT NULL, 
    [KeyPhrases] TEXT NOT NULL, 
    CONSTRAINT [FK_VideoCommentAnalysis_VideoComment] FOREIGN KEY ([VideoCommentId]) REFERENCES [VideoComment]([VideoCommentId]) 
)

GO

CREATE UNIQUE INDEX [UI_VideoCommentAnalysis_VideoCommentId] ON [dbo].[VideoCommentAnalysis] ([VideoCommentId])
