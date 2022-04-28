USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetStatsFixture')
DROP PROCEDURE dbo.spGetStatsFixture 
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 22 Nov 2019
-- Description:	Get details for the groups game stats page
-- =============================================
-- EXEC predictioncomp.dbo.spGetStatsFixture 3488, 212, 0
CREATE PROCEDURE dbo.spGetStatsFixture 
(
	@intFixtureId INT
	, @intEventId INT
	, @intPoolId INT = 0
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	IF (@intPoolId = 0)
	BEGIN

		SELECT FP.HomePrediction
			, FP.AwayPrediction
			, CAST(COUNT(FP.PlayerID) AS int) AS NumberOfPredictions
		FROM dbo.FixturePredictions AS FP
		WHERE FP.FixtureId = @intFixtureId
		AND FP.EventId = @intEventId
		GROUP BY FixtureId
			, FP.HomePrediction
			, FP.AwayPrediction
		ORDER BY CASE WHEN FP.HomePrediction > FP.AwayPrediction THEN 1
				WHEN FP.HomePrediction = FP.AwayPrediction THEN 2
				WHEN FP.AwayPrediction > FP.HomePrediction THEN 3
				END
		, ABS(FP.HomePrediction) - ABS(FP.AwayPrediction);

	END
	ELSE
	BEGIN

		SELECT FP.HomePrediction
			, FP.AwayPrediction
			, CAST(COUNT(FP.PlayerID) AS int) AS NumberOfPredictions
		FROM dbo.FixturePredictions AS FP
		INNER JOIN dbo.EventFixtures EF ON EF.FixtureId = FP.FixtureId AND EF.EventId = FP.EventId
		INNER JOIN dbo.EventPoolPlayers EPP ON EPP.PoolId = @intPoolId AND EPP.EventId = EF.EventId AND EPP.Enabled = 1 AND EPP.PlayerId = FP.PlayerId
		WHERE FP.FixtureId = @intFixtureId
		AND FP.EventId = @intEventId
		GROUP BY FP.FixtureId
			, FP.HomePrediction
			, FP.AwayPrediction
		ORDER BY CASE WHEN FP.HomePrediction > FP.AwayPrediction THEN 1
				WHEN FP.HomePrediction = FP.AwayPrediction THEN 2
				WHEN FP.AwayPrediction > FP.HomePrediction THEN 3
				END
		, ABS(FP.HomePrediction) - ABS(FP.AwayPrediction);

	END

	END
GO