USE [predict]
GO

/****** Object:  Table [dbo].[Event]    Script Date: 06/01/2019 22:11:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Event]
(
	[EventID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[EventName] [varchar](50) NOT NULL,
	[EventStartDateTime] DATETIME NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[ModifiedDateTime] [datetime] NOT NULL
) 

GO

SET ANSI_PADDING OFF
GO


