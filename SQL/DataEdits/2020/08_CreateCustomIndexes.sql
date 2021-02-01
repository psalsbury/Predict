USE [Predictioncomp]
GO

/****** Object:  Index [IX_RapidApiFixtureId]    Script Date: 01/02/2021 21:08:01 ******/
CREATE NONCLUSTERED INDEX [IX_RapidApiFixtureId] ON [dbo].[Fixtures]
(
	[RapidApiFixtureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


