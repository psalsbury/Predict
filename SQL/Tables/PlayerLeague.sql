USE [predict]
GO

/****** Object:  Table [dbo].[PlayerPool]    Script Date: 06/01/2019 21:55:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayerPool]
(
	[PlayerLeagueID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PlayerID] [int] NOT NULL,
	[PoolID] [int] NOT NULL,
	[AdminApprovedDateTime] DATETIME NULL,
	[FinalGoalMinute] [int] NULL,
	[Posn] [int] NULL,
	[CorrectScores] [int] NULL,
	[CorrectResults] [int] NULL,
	[WinMargin] [int] NULL,
	[KOScore] [money] NULL,
	[BonusScore] [int] NULL,
	[TotalScore] [int] NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[ModifiedDateTime] [datetime] NOT NULL
 )
GO


