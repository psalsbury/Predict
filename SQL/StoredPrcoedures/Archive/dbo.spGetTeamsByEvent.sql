USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetTeamsByEvent')
DROP PROCEDURE dbo.spGetTeamsByEvent 
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 24 Sep 2020
-- Description:	Get Teams By Event
-- =============================================
-- EXEC dbo.spGetTeamsByEvent 1
CREATE PROCEDURE dbo.spGetTeamsByEvent 
(
	@intEventId INT
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT DISTINCT TH.ID AS TeamId		
		, TH.TeamName
		, F.LeagueId
		, LE.LeagueName
		, LE.ShortLeagueName
		, TH.[TeamFlag] AS FlagFileLocation
	FROM [dbo].[EventFixtures] AS EF 
	INNER JOIN [dbo].[Fixtures] AS F ON F.Id = EF.FixtureId
	INNER JOIN [dbo].[Leagues] AS LE ON LE.Id = F.LeagueId	
	INNER JOIN [dbo].[Teams] AS TH ON TH.Id = F.[HomeTeamId] OR TH.Id = F.[AwayTeamId]
	WHERE EF.EventId = @intEventId

END
GO