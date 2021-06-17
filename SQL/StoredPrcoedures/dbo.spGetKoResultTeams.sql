USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetKoResultTeams')
DROP PROCEDURE dbo.spGetKoResultTeams 
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 17 Jun 2021
-- Description:	Get Teams that are in the KO stage
-- =============================================
-- EXEC dbo.spGetKoResultTeams 1
CREATE PROCEDURE dbo.spGetKoResultTeams 
(
	@intEventId INT
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT KO.RoundOf
	, KO.Team1Id AS TeamId	
	FROM dbo.KoFixtures AS KO
	WHERE KO.EventId = @intEventId
	AND KO.Team1Id IS NOT NULL

	UNION ALL

	SELECT KO.RoundOf
		, KO.Team2Id AS TeamId			
	FROM dbo.KoFixtures AS KO
	WHERE KO.EventId = @intEventId
	AND KO.Team2Id IS NOT NULL

	UNION ALL
		
	SELECT 1 
		, KOE.WinningTeamId AS TeamId	
	FROM dbo.EventsKo AS KOE
	WHERE KOE.EventId = @intEventId
	AND KOE.WinningTeamId IS NOT NULL;

END
GO