use Predict

/****** Object:  Table [dbo].[Player]    Script Date: 06/01/2019 21:44:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Player]
(
	[PlayerID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Email] [varchar](100) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Name] [varchar](30) NOT NULL,
	[DisplayName] [varchar](30) NOT NULL,
	[SupportTeamId] [varchar](50) NULL,
	[ActivationCode] [varchar](50) NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[ModifiedDateTime] [datetime] NOT NULL,
	[ActivatedDateTime] [datetime] NULL
)
GO



