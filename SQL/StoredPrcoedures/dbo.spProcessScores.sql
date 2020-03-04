USE predict
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spProcessScores')
DROP PROCEDURE dbo.spProcessScores 
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 17 Feb 2019
-- Description:	Calculate all the scores
-- =============================================
-- EXEC dbo.spProcessScores 1, '20 jan 2020'
CREATE PROCEDURE dbo.spProcessScores 
(
	@intEventId INT 
	, @dteDate DATE
)
AS
BEGIN

	SET NOCOUNT ON;
	
	/* Clear down the fixture prediction row */
	UPDATE FP
	SET [CorrectScore] = NULL
		, [CorrectResult] = NULL
		, [CorrectWinMargin] = NULL
	FROM dbo.FixturePredictions AS FP
	INNER JOIN dbo.Fixtures AS FI ON FI.Id = FP.FixtureId
	WHERE FI.EventId = @intEventId;

	/* Clear down all the pool scores */
	UPDATE PP
	SET PP.[CorrectScore] = 0
		, PP.CorrectResult = 0
		, PP.WinMargin = 0
		, KoScore = 0
		, BonusScore = 0
		, TotalScore = 0
		, ModifiedDateTime = GETDATE()
	FROM PoolPlayers AS PP
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
	WHERE PO.EventId = @intEventId;
		
	/* Calculate the group scores */
	UPDATE FP
	SET	 FP.CorrectScore =	CASE		WHEN FP.HomePrediction = FX.HomeResult AND FP.AwayPrediction = FX.AwayResult 
										THEN 1	/* Correct score predicted */
										ELSE 0
								END 
		,FP.CorrectResult =		CASE	WHEN FP.HomePrediction = FX.HomeResult AND FP.AwayPrediction = FX.AwayResult 
										THEN 0 /* Correct score predicted, so points already accumulated */
										WHEN (FP.HomePrediction > FP.AwayPrediction AND FX.HomeResult > FX.AwayResult) 
											OR (FP.HomePrediction = FP.AwayPrediction AND FX.HomeResult = FX.AwayResult)
											OR (FP.HomePrediction < FP.AwayPrediction AND FX.HomeResult < FX.AwayResult)
										THEN 1 /* Correct score predicted */
										ELSE 0
								END
		,FP.CorrectWinMargin =	CASE	WHEN FP.HomePrediction = FX.HomeResult AND FP.AwayPrediction = FX.AwayResult 
										THEN 0 /* Correct score predicted, so points already accumulated */
										WHEN ((FP.HomePrediction - FP.AwayPrediction) = (FX.HomeResult - FX.AwayResult)) AND FP.HomePrediction <> FP.AwayPrediction 
										THEN 1 
										ELSE 0
								END
		, FP.ModifiedDateTime = GETDATE()
	FROM dbo.FixturePredictions AS FP
	INNER JOIN dbo.Fixtures AS FX WITH (NOLOCK) ON FX.Id = FP.FixtureId
	WHERE FX.HomeResult IS NOT NULL 
	AND FX.AwayResult IS NOT NULL
	AND FX.EventId = @intEventId;

	CREATE TABLE #FixturePredictionScores
	(
		FixturePredictionId INT
		,PoolId INT
		,CorrectScorePoints INT
		,CorrectResultPoints INT
		,CorrectWinMarginPoints INT
	)

	INSERT INTO #FixturePredictionScores
	(
		FixturePredictionId
		,PoolId
		,CorrectScorePoints
		,CorrectResultPoints
		,CorrectWinMarginPoints
	)
	SELECT	FP.Id AS FixturePredictionId
			, PO.Id AS PoolId
			, CASE WHEN FP.CorrectScore = 1 THEN PO.CorrectScorePoints ELSE 0 END AS CorrectScorePoints
			, CASE WHEN FP.CorrectResult = 1 THEN PO.CorrectResultPoints ELSE 0 END AS CorrectResult
			, CASE WHEN FP.CorrectWinMargin = 1 THEN PO.WinMarginPoints ELSE 0 END AS CorrectWinMarginPoints
	FROM dbo.FixturePredictions AS FP 
	INNER JOIN dbo.PoolPlayers AS PP ON PP.PlayerId = FP.PlayerId
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
	WHERE PO.EventId = @intEventId;

	/* Update the PoolPlayer table with a summary of the fixture scores for each player/pool */
	WITH CTE AS
	(
		SELECT FPP.PoolId
			, FP.PlayerId
			, SUM(FPP.CorrectScorePoints) AS CorrectScore
			, SUM(FPP.CorrectResultPoints) AS CorrectResult
			, SUM(FPP.CorrectWinMarginPoints) AS WinMargin
		FROM #FixturePredictionScores AS FPP
		INNER JOIN dbo.FixturePredictions AS FP ON FP.Id = FPP.FixturePredictionId
		GROUP BY FPP.PoolId
			, FP.PlayerId
	)
	UPDATE PP
	SET PP.CorrectScore = CTE.CorrectScore
		, PP.[CorrectResult] = CTE.CorrectResult
		, PP.[WinMargin] = CTE.WinMargin
	FROM dbo.PoolPlayers AS PP
	INNER JOIN CTE ON CTE.PoolId = PP.PoolId AND CTE.PlayerId = PP.PlayerId;
	
	
	/* Calculate the KO scores */
	CREATE TABLE #tmpKOPredictions
	(
		Id INT IDENTITY(1,1)
		, PlayerID NVARCHAR(128)		
		, RoundOf SMALLINT
		, TeamId INT
	)

	/* Get list of unique teams in each round per player */
	INSERT INTO #tmpKOPredictions
	(
		  KOFP.PlayerId
		, KOF.RoundOf
		, TeamId
	)
	SELECT KOFP.PlayerId
		, KOF.RoundOf
		, KOFP.Team1Id		
	FROM dbo.KoFixturePredictions AS KOFP
	INNER JOIN dbo.KoFixtures AS KOF ON KOF.Id = KOFP.KoFixtureId
	WHERE KOF.EventId = @intEventId
	UNION ALL
	SELECT KOFP.PlayerId
		, KOF.RoundOf
		, KOFP.Team2Id	
	FROM dbo.KoFixturePredictions AS KOFP
	INNER JOIN dbo.KoFixtures AS KOF ON KOF.Id = KOFP.KoFixtureId
	WHERE KOF.EventId = @intEventId
	UNION ALL
	SELECT KOW.PlayerId
		, 1 
		, KOW.TeamId
	FROM dbo.KoWinningTeamPredictions AS KOW
	WHERE KOW.EventId = @intEventId;

	/* De- Dupe any teams that are in the same round more than once */
	WITH CTE AS
	(
		SELECT MIN(Id) MinId
			, PlayerId
			, RoundOf
			, TeamId
		FROM #tmpKOPredictions
		GROUP BY PlayerId
			, RoundOf
			, TeamId
	)
	DELETE KOP
	FROM #tmpKOPredictions KOP
	INNER JOIN CTE ON CTE.PlayerID = KOP.PlayerID AND CTE.TeamId=KOP.TeamId AND CTE.RoundOf = KOP.RoundOf AND KOP.Id <> CTE.MinId;
	
	CREATE TABLE #koResults
	(
		RoundOf INT
		, TeamId INT
	)

	INSERT INTO #koResults
	(
		RoundOf
		, TeamId
	)
	SELECT KOF.RoundOf
		, KOF.Team1Id
	FROM dbo.KoFixtures AS KOF
	WHERE KOF.EventId = @intEventId
	AND KOF.Team1Id IS NOT NULL
	UNION ALL
	SELECT KOF.RoundOf
		, KOF.Team2Id
	FROM dbo.KoFixtures AS KOF
	WHERE KOF.EventId = @intEventId
	AND KOF.Team2Id IS NOT NULL
	UNION ALL 
	SELECT 1
		, EVKO.WinningTeamId
	FROM dbo.EventsKo AS EVKO
	WHERE EVKO.EventId = @intEventId
	AND EVKO.WinningTeamId IS NOT NULL;

	CREATE TABLE #tmpKO
	(
		PlayerID NVARCHAR(128)
		, PoolId INT
		, RoundOf INT
		, TeamsCorrect INT
		, RoundOfScore INT
	)

	INSERT INTO #tmpKO
	(
		PlayerID
		, PoolId
		, RoundOf
		, TeamsCorrect
		, RoundOfScore
	)
	SELECT KOP.PlayerID
		, PP.PoolId
		, KOP.RoundOf
		, COUNT(KOR.TeamId)
		, CASE	WHEN KOP.RoundOf = 16 THEN PO.KoLast16Points
				WHEN KOP.RoundOf = 8 THEN PO.KoLast8Points
				WHEN KOP.RoundOf = 4 THEN PO.KoLast4Points
				WHEN KOP.RoundOf = 2 THEN PO.KoLast2Points
				WHEN KOP.RoundOf = 1 THEN PO.KoLast1Points
				ELSE 0 
			END AS RoundOfScore
	FROM #tmpKOPredictions AS KOP
	INNER JOIN #koResults KOR ON KOR.RoundOf = KOP.RoundOf AND KOR.TeamId = KOP.TeamId
	INNER JOIN dbo.PoolPlayers AS PP ON PP.PlayerId = KOP.PlayerID
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId 
	WHERE PO.EventId = @intEventId
	GROUP BY KOP.PlayerID
		, PP.PoolId
		, KOP.RoundOf
		, PO.KoLast16Points
		, PO.KoLast8Points
		, PO.KoLast4Points
		, PO.KoLast2Points
		, PO.KoLast1Points;

	WITH CTE AS
	(
		SELECT PlayerID
			, PoolId
			, SUM(TeamsCorrect*RoundOfScore) AS KoScore
		FROM #tmpKO
		GROUP BY PlayerID
			, PoolId
	)
	UPDATE PP
	SET KoScore = CTE.KoScore
	FROM dbo.PoolPlayers AS PP
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId 
	INNER JOIN CTE ON CTE.PlayerID = PP.PlayerId AND CTE.PoolId = PP.PoolId
	WHERE PO.EventId = @intEventId;

	/* Update each PlayerPool record with the total score and the position within the league */
	WITH CTE AS
	(
		SELECT PP.PoolId
			, PP.PlayerId
			, PP.CorrectScore+PP.CorrectResult+PP.WinMargin+PP.KoScore+PP.BonusScore AS TotalScore
			, ROW_NUMBER() OVER(PARTITION BY PO.EventId, PP.PoolId
					ORDER BY PP.CorrectScore+PP.CorrectResult+PP.WinMargin+PP.KoScore+PP.BonusScore DESC
							, PP.CorrectScore DESC
							, PP.KoScore DESC
							, PL.CreatedDateTime) AS PoolPosition
		FROM dbo.PoolPlayers AS PP
		INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
		INNER JOIN dbo.Players AS PL ON PL.Id = PP.PlayerId
		WHERE PO.EventId = @intEventId
	)
	UPDATE PP
	SET PP.PoolPosition = CTE.PoolPosition
		, PP.TotalScore = CTE.TotalScore
		, PP.ModifiedDateTime = GETDATE()
	FROM dbo.PoolPlayers AS PP
	INNER JOIN CTE ON CTE.PoolId = PP.PoolId AND CTE.PlayerId = PP.PlayerId;

	/* Update the position history */
	UPDATE PPPH
	SET PPPH.PoolPosition = PP.PoolPosition
		, PPPH.ModifiedDateTime = GETDATE()
	FROM dbo.PoolPlayerPositionHistory AS PPPH
	INNER JOIN dbo.PoolPlayers AS PP ON PP.PoolId = PPPH.PoolId AND PP.PlayerId = PPPH.PlayerId
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
	WHERE PPPH.PositionDate = @dteDate
	AND PO.EventId = @intEventId

	INSERT INTO dbo.PoolPlayerPositionHistory
	(
		PoolId
		, PlayerId
		, PositionDate
		, PoolPosition
		, CreatedDateTime
		, ModifiedDateTime
	)
	SELECT PP.PoolId
		, PP.PlayerId
		, @dteDate
		, PP.PoolPosition
		, GETDATE()
		, GETDATE()
	FROM dbo.PoolPlayers AS PP
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
	LEFT OUTER JOIN dbo.PoolPlayerPositionHistory AS PPPH ON PPPH.PlayerId = PP.PlayerId AND PPPH.PoolId = PP.PoolId AND PPPH.PositionDate = @dteDate
	WHERE PO.EventId = @intEventId
	AND PPPH.Id IS NULL;

END
GO
