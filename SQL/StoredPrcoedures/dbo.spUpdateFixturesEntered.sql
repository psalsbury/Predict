USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spUpdateFixturesEntered')
DROP PROCEDURE dbo.spUpdateFixturesEntered
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 09 Jun 2021
-- Description:	Update number of fixtures entered
-- =============================================
-- EXEC predictioncomp.dbo.spUpdateFixturesEntered 'e51699d7-7cf2-4565-905c-4a89c4f80063'
CREATE PROCEDURE dbo.spUpdateFixturesEntered
(
	@strPlayerId NVARCHAR(128)
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	WITH CTE AS
	(
		SELECT PlayerId
			, EventId
			, COUNT(1) AS NbrPredsEntered
		FROM FixturePredictions FP
		WHERE FP.PlayerID = @strPlayerId
		GROUP BY FP.PlayerId
			, EventId
	)
	UPDATE EPP
	SET EPP.FixturePredictionsEntered = CTE.NbrPredsEntered
		, EPP.ModifiedDateTime = GETUTCDATE()
	FROM dbo.EventPoolPlayers AS EPP
	INNER JOIN CTE ON CTE.EventId = EPP.EventId AND CTE.PlayerId = EPP.PlayerId;

	WITH CTE AS
	(
		SELECT PlayerId
				, EventId
				, SUM(KOPredsEntered) AS NbrKoPredsEntered
		FROM (
				SELECT PlayerId
						, EventId
						, SUM(2) KOPredsEntered
					FROM [dbo].[KoFixturePredictions] KOFP
					INNER JOIN [dbo].[KoFixtures] KOF ON KOF.Id = KOFP.KoFixtureId
					WHERE KOFP.PlayerID = @strPlayerId
					GROUP BY PlayerId
						, EventId

					UNION ALL

					SELECT PlayerId
						, EventId
						, 1 AS KOPredsEntered
					FROM [dbo].[KoWinningTeamPredictions] AS WIN
					WHERE WIN.PlayerID = @strPlayerId
				) AS SUB
		GROUP BY PlayerId
				, EventId
	)
	UPDATE EPP
	SET EPP.KoPredictionsEntered = CTE.NbrKoPredsEntered
		, EPP.ModifiedDateTime = GETUTCDATE()
	FROM EventPoolPlayers AS EPP
	INNER JOIN CTE ON CTE.EventId = EPP.EventId AND CTE.PlayerId = EPP.PlayerId

END
GO