USE [predict]
GO

/****** Object:  Table [dbo].[Pool]    Script Date: 06/01/2019 21:55:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Pool]
(
	[PoolId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PoolName] [varchar](50) NOT NULL,
	[AdminPlayerID] [int] NOT NULL,
	[JoinCode] varchar(20) NULL,
	[InitialInfo] VARCHAR(8000) NULL,
	[MemberInfo] VARCHAR(8000) NULL,
	[FreezePredictions] BIT NOT NULL,
	[EntryFee] MONEY NULL,
	[FirstPercent] DECIMAL(10,3) NULL,
	[SecondPercent] DECIMAL(10,3) NULL,
	[ThirdPercent] DECIMAL(10,3) NULL,
	[NonPrizePercent] DECIMAL(10,3) NULL,
	[EmailNotifications] BIT NOT NULL,
	[CorrectScorePoints] SMALLINT NOT NULL,
	[CorrectResultPoints] SMALLINT NOT NULL,
	[WinMarginPoints] SMALLINT NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[ModifiedDateTime] [datetime] NOT NULL
)

