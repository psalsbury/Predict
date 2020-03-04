USE [predict]
GO

/****** Object:  Table [dbo].[[FixturePrediction]]    Script Date: 06/01/2019 22:11:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FixturePrediction]
(
	[FixturePredictionID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FixtureId] INT NOT NULL,
	[PlayerId] INT NOT NULL,
	[HomeScore] SMALLINT NOT NULL,
	[AwayScore] SMALLINT NOT NULL,
	[LuckyDip] BIT NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[ModifiedDateTime] [datetime] NOT NULL
) 

GO

SET ANSI_PADDING OFF
GO


