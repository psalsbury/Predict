USE predictioncomp
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
-- EXEC dbo.spProcessScores '23 sep 2020'
CREATE PROCEDURE dbo.spProcessScores 
(
	@dteDate DATE
)
AS
BEGIN

	SET NOCOUNT ON;

	CREATE TABLE #tmpEventPools
	(
		EventID INT
		, PoolId INT
	)

	CREATE TABLE #tmpEvents
	(
		EventID INT
	)
	
	/* Find all the EventPool entries with an outstanding result to process */
	INSERT INTO #tmpEventPools
	(
		EventID 
		, PoolId 
	)
	SELECT DISTINCT EF.EventID
		, EP.PoolId
	FROM [dbo].[Fixtures] AS F
	INNER JOIN [dbo].[EventFixtures] AS EF ON EF.FixtureId = F.Id
	INNER JOIN [dbo].[EventPools] AS EP ON EP.EventId = EF.EventId
	WHERE F.ResultProcessed = 0
	AND F.HomeResult IS NOT NULL
	AND F.AwayResult IS NOT NULL

	UNION 

	SELECT DISTINCT KO.EventID
		, EP.PoolId
	FROM [dbo].[KoFixtures] AS KO
	INNER JOIN [dbo].[EventPools] AS EP ON EP.EventId = KO.EventId
	WHERE KO.ResultProcessed = 0
	AND KO.Team1Id IS NOT NULL
	OR KO.Team2Id IS NOT NULL

	INSERT INTO #tmpEvents
	(EventId)
	SELECT DISTINCT EventID
	FROM #tmpEventPools;

	/* Clear down the fixture prediction row */
	UPDATE FP
	SET [CorrectScore] = NULL
		, [CorrectResult] = NULL
		, [CorrectWinMargin] = NULL
	FROM dbo.FixturePredictions AS FP
	INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = FP.EventId;

	/* Clear down all the pool scores */
	UPDATE PP
	SET PP.[CorrectScore] = 0
		, PP.CorrectResult = 0
		, PP.WinMargin = 0
		, KoScore = 0
		, BonusScore = 0
		, TotalScore = 0
		, ModifiedDateTime = GETDATE()
	FROM EventPoolPlayers AS PP
	INNER JOIN #tmpEventPools AS TMP ON TMP.EventId = PP.EventId AND TMP.PoolId = PP.PoolId;
		
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
	INNER JOIN dbo.Fixtures AS FX ON FX.Id = FP.FixtureId
	INNER JOIN #tmpEvents AS TMP ON TMP.EventId = FP.EventId
	WHERE FX.HomeResult IS NOT NULL 
	AND FX.AwayResult IS NOT NULL;

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
	INNER JOIN #tmpEventPools AS TMP ON TMP.EventId = FP.EventId
	INNER JOIN dbo.Pools AS PO ON PO.Id = TMP.PoolId;

	/* Update the PoolPlayer table with a summary of the fixture scores for each player/pool */
	WITH CTE AS
	(
		SELECT FP.EventId
			, FPP.PoolId
			, FP.PlayerId
			, SUM(FPP.CorrectScorePoints) AS CorrectScore
			, SUM(FPP.CorrectResultPoints) AS CorrectResult
			, SUM(FPP.CorrectWinMarginPoints) AS WinMargin
		FROM #FixturePredictionScores AS FPP
		INNER JOIN dbo.FixturePredictions AS FP ON FP.Id = FPP.FixturePredictionId
		GROUP BY FP.EventId
			, FPP.PoolId
			, FP.PlayerId
	)
	UPDATE PP
	SET PP.CorrectScore = CTE.CorrectScore
		, PP.[CorrectResult] = CTE.CorrectResult
		, PP.[WinMargin] = CTE.WinMargin
		, PP.ModifiedDateTime = GETDATE()
	FROM dbo.EventPoolPlayers AS PP
	INNER JOIN CTE ON CTE.EventId = PP.EventId AND CTE.PoolId = PP.PoolId AND CTE.PlayerId = PP.PlayerId;
	
	IF EXISTS(SELECT 1 FROM [dbo].[EventsKo] WHERE EventId IN (SELECT EventID FROM #tmpEvents))
	BEGIN
	
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
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = KOF.EventId

		UNION ALL

		SELECT KOFP.PlayerId
			, KOF.RoundOf
			, KOFP.Team2Id	
		FROM dbo.KoFixturePredictions AS KOFP
		INNER JOIN dbo.KoFixtures AS KOF ON KOF.Id = KOFP.KoFixtureId
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = KOF.EventId

		UNION ALL
		
		SELECT KOW.PlayerId
			, 1 
			, KOW.TeamId
		FROM dbo.KoWinningTeamPredictions AS KOW
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = KOW.EventId;

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
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = KOF.EventId
		AND KOF.Team1Id IS NOT NULL
		UNION ALL
		SELECT KOF.RoundOf
			, KOF.Team2Id
		FROM dbo.KoFixtures AS KOF
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = KOF.EventId
		AND KOF.Team2Id IS NOT NULL
		UNION ALL 
		SELECT 1
			, EVKO.WinningTeamId
		FROM dbo.EventsKo AS EVKO
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = EVKO.EventId
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
		INNER JOIN dbo.EventPoolPlayers AS PP ON PP.PlayerId = KOP.PlayerID
		INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId 
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = PP.EventId
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
		FROM dbo.EventPoolPlayers AS PP
		INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId 
		INNER JOIN CTE ON CTE.PlayerID = PP.PlayerId AND CTE.PoolId = PP.PoolId
		INNER JOIN #tmpEvents AS TMP ON TMP.[EventId] = PP.EventId;
	END;

	/* Update each PlayerPool record with the total score and the position within the league */
	WITH CTE AS
	(
		SELECT PP.EventId
			, PP.PoolId
			, PP.PlayerId
			, PP.CorrectScore+PP.CorrectResult+PP.WinMargin+PP.KoScore+PP.BonusScore AS TotalScore
			, ROW_NUMBER() OVER(PARTITION BY PP.EventId, PP.PoolId
					ORDER BY PP.CorrectScore+PP.CorrectResult+PP.WinMargin+PP.KoScore+PP.BonusScore DESC
							, PP.CorrectScore DESC
							, PP.KoScore DESC
							, PL.CreatedDateTime) AS PoolPosition
		FROM dbo.EventPoolPlayers AS PP
		INNER JOIN #tmpEventPools AS TMP ON TMP.EventId = PP.EventId AND TMP.PoolId = PP.PoolId
		INNER JOIN dbo.Players AS PL ON PL.Id = PP.PlayerId
	)
	UPDATE PP
	SET PP.PoolPosition = CTE.PoolPosition
		, PP.TotalScore = CTE.TotalScore
		, PP.ModifiedDateTime = GETDATE()
	FROM dbo.EventPoolPlayers AS PP
	INNER JOIN CTE ON CTE.EventId = PP.EventId AND CTE.PoolId = PP.PoolId AND CTE.PlayerId = PP.PlayerId;

	/* Update the position history */
	UPDATE PPPH
	SET PPPH.PoolPosition = PP.PoolPosition
		, PPPH.ModifiedDateTime = GETDATE()
	FROM dbo.EventPoolPlayerPositionHistory AS PPPH
	INNER JOIN dbo.EventPoolPlayers AS PP ON PP.PoolId = PPPH.PoolId AND PP.PlayerId = PPPH.PlayerId
	INNER JOIN #tmpEventPools AS TMP ON TMP.EventId = PP.EventId AND TMP.PoolId = PP.PoolId
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
	WHERE PPPH.PositionDate = @dteDate

	INSERT INTO dbo.EventPoolPlayerPositionHistory
	(
		EventId
		, PoolId
		, PlayerId
		, PositionDate
		, PoolPosition
		, CreatedDateTime
		, ModifiedDateTime
	)
	SELECT PP.EventId
		, PP.PoolId
		, PP.PlayerId
		, @dteDate
		, PP.PoolPosition
		, GETDATE()
		, GETDATE()
	FROM dbo.EventPoolPlayers AS PP
	INNER JOIN #tmpEventPools AS TMP ON TMP.EventId = PP.EventId AND TMP.PoolId = PP.PoolId
	INNER JOIN dbo.Pools AS PO ON PO.Id = PP.PoolId
	LEFT OUTER JOIN dbo.EventPoolPlayerPositionHistory AS PPPH ON PPPH.PlayerId = PP.PlayerId AND PPPH.PoolId = PP.PoolId AND PPPH.PositionDate = @dteDate
	WHERE PPPH.PlayerId IS NULL;

	/* Update Fixtures to be processed */
	UPDATE FX
	SET FX.ResultProcessed = 1
		, FX.ModifiedDateTime = GETDATE()
	FROM [dbo].[Fixtures] AS FX
	WHERE FX.ResultProcessed = 0
	AND FX.HomeResult IS NOT NULL
	AND FX.AwayResult IS NOT NULL;

	UPDATE KO
	SET	KO.ResultProcessed = 1
		, KO.ModifiedDateTime = GETDATE()
	FROM [dbo].[KoFixtures] AS KO
	WHERE KO.ResultProcessed = 0
	AND KO.Team1Id IS NOT NULL
	AND KO.Team2Id IS NOT NULL

	/* Update LastModifiedDateTime for this event */
	UPDATE EV
	SET EV.ModifiedDateTime = GETDATE()
	FROM dbo.[Events] AS EV
	INNER JOIN #tmpEvents AS TMP ON TMP.EventId = EV.Id;

END
GO
