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
-- EXEC predict.dbo.spGetStatsFixture 1
CREATE PROCEDURE dbo.spGetStatsFixture 
(
	@intFixtureId INT
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT FP.HomePrediction
		, FP.AwayPrediction
		, CAST(COUNT(FP.PlayerID) AS int) AS NumberOfPredictions
	FROM dbo.FixturePredictions AS FP
	WHERE FP.FixtureId = @intFixtureId
	GROUP BY FixtureId
		, FP.HomePrediction
		, FP.AwayPrediction
	ORDER BY CASE WHEN FP.HomePrediction > FP.AwayPrediction THEN 1
			WHEN FP.HomePrediction = FP.AwayPrediction THEN 2
			WHEN FP.AwayPrediction > FP.HomePrediction THEN 3
			END
	, ABS(FP.HomePrediction) - ABS(FP.AwayPrediction);

	END
GO