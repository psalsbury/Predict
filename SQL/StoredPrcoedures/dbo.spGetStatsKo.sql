USE predictioncomp
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
-- EXEC predictioncomp.dbo.spGetStatsKo 1, 1
CREATE PROCEDURE dbo.spGetStatsKo 
(
	@intEventId INT
	, @intPoolId INT = 0
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	DECLARE @tblPlayers TABLE (PlayerId NVARCHAR(128));

	INSERT INTO @tblPlayers
	SELECT DISTINCT EPP.PlayerId
	FROM EventPoolPlayers AS EPP
	WHERE EPP.PoolId = CASE WHEN @intPoolId = 0 THEN EPP.PoolId ELSE @intPoolId END
	AND EPP.Enabled = 1;

	WITH CTE AS
	(
		SELECT KOFP.Team1Id AS TeamId
			, T.TeamName
			, KO.RoundOf
			, COUNT(1) AS NumberOfPredictions
		FROM dbo.KoFixturePredictions AS KOFP
		INNER JOIN dbo.KoFixtures AS KO ON KO.Id = KOFP.KoFixtureId
		INNER JOIN dbo.Teams AS T ON T.Id = KOFP.Team1Id
		INNER JOIN @tblPlayers AS TMP ON TMP.PlayerId = KOFP.PlayerId
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
		INNER JOIN @tblPlayers AS TMP ON TMP.PlayerId = KOFP.PlayerId
		WHERE KO.EventId = @intEventId
		GROUP BY KOFP.Team2Id, KO.RoundOf, T.TeamName
		UNION ALL
		SELECT KOWT.TeamId
			, T.TeamName
			, 1
			, COUNT(1) AS NumberOfPredictions
		FROM dbo.KoWinningTeamPredictions AS KOWT
		INNER JOIN dbo.Teams AS T ON T.Id = KOWT.TeamId
		INNER JOIN @tblPlayers AS TMP ON TMP.PlayerId = KOWT.PlayerId
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