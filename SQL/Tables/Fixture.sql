USE [predict]
GO

/****** Object:  Table [dbo].[Fixture]    Script Date: 06/01/2019 22:11:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fixture]
(
	[FixtureID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[EventId] INT,
	[FixtureDateTime] DATETIME,
	[HomeTeamId] INT,
	[AwayTeamId] INT,
	[HomeResult] SMALLINT,
	[AwayResult] SMALLINT,
	[CreatedDateTime] [datetime] NOT NULL,
	[ModifiedDateTime] [datetime] NOT NULL
) 

GO

SET ANSI_PADDING OFF
GO


