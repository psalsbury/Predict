USE predict
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetStatsKo')
DROP PROCEDURE dbo.spGetStatsKo 
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 24 Nov 2019
-- Description:	Get details for the Ko Games
-- =============================================
-- EXEC predict.dbo.spGetStatsKo 1
CREATE PROCEDURE dbo.spGetStatsKo 
(
	@intEventId INT
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	WITH CTE AS
	(
		SELECT KOFP.Team1Id AS TeamId
			, T.TeamName
			, KO.RoundOf
			, COUNT(1) AS NumberOfPredictions
		FROM dbo.KoFixturePredictions AS KOFP
		INNER JOIN dbo.KoFixtures AS KO ON KO.Id = KOFP.KoFixtureId
		INNER JOIN dbo.Teams AS T ON T.Id = KOFP.Team1Id
		WHERE KO.EventId = @intEventId
		GROUP BY KOFP.Team1Id, KO.RoundOf, T.TeamName
		UNION ALL 
		SELECT KOFP.Team2Id AS TeamId
			, T.TeamName
			, KO.RoundOf
			, COUNT(1) AS NumberOfPredictions
		FROM KoFixturePredictions AS KOFP
		INNER JOIN KoFixtures AS KO ON KO.Id = KOFP.KoFixtureId
		INNER JOIN dbo.Teams AS T ON T.Id = KOFP.Team2Id
		WHERE KO.EventId = @intEventId
		GROUP BY KOFP.Team2Id, KO.RoundOf, T.TeamName
		UNION ALL
		SELECT KOWT.TeamId
			, T.TeamName
			, 1
			, COUNT(1) AS NumberOfPredictions
		FROM dbo.KoWinningTeamPredictions AS KOWT
		INNER JOIN dbo.Teams AS T ON T.Id = KOWT.TeamId
		WHERE KOWT.EventId = @intEventId
		GROUP BY KOWT.TeamId, T.TeamName
	)
	SELECT CTE.RoundOf
		, CTE.TeamId
		, CTE.TeamName
		, SUM(CTE.NumberOfPredictions) AS NumberOfPredictions
	FROM CTE AS CTE
	GROUP BY CTE.RoundOf
		, CTE.TeamId
		, CTE.TeamName


END
GO