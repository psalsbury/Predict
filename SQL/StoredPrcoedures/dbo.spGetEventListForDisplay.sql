USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetEventListForDisplay')
DROP PROCEDURE dbo.spGetEventListForDisplay 
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 12 May 2022
-- Description:	Get next fixture to get the result
-- =============================================
-- EXEC predictioncomp.dbo.spGetEventListForDisplay 1
-- EXEC predictioncomp.dbo.spGetEventListForDisplay 1, 'e51699d7-7cf2-4565-905c-4a89c4f80063'
-- EXEC predictioncomp.dbo.spGetEventListForDisplay 1, NULL, 'e51699d7-7cf2-4565-905c-4a89c4f80063'
-- EXEC predictioncomp.dbo.spGetEventListForDisplay 0, NULL, 'e51699d7-7cf2-4565-905c-4a89c4f80063'
CREATE PROCEDURE dbo.spGetEventListForDisplay 
(
	@OnlyShowActive BIT = 0
	, @CreatedByUserId NVARCHAR(128) = NULL
	, @ParticipatingInUserId NVARCHAR(128) = NULL
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	DECLARE @tmp TABLE
	(EventId SMALLINT)

	IF (@CreatedByUserId IS NOT NULL)
	BEGIN
		INSERT INTO @tmp
		SELECT E.Id
		FROM dbo.[Events] AS E
		WHERE E.CreatedByPlayerId = @CreatedByUserId
		AND CASE WHEN @OnlyShowActive = 1 
				 THEN 
					CASE WHEN E.EndDateTime > GETUTCDATE() THEN 1 ELSE 0 END
				 ELSE 
				 	CASE WHEN E.EndDateTime <= GETUTCDATE() THEN 1 ELSE 0 END
				 END = 1
	END

	IF (@ParticipatingInUserId IS NOT NULL)
	BEGIN
		INSERT INTO @tmp
		SELECT EP.EventId
		FROM dbo.EventPlayers  AS EP
		INNER JOIN dbo.[Events] AS E ON E.Id = EP.EventId
		LEFT OUTER JOIN @tmp AS T ON T.EventId = EP.EventId
		WHERE EP.PlayerId = @ParticipatingInUserId
		AND EP.[Enabled] = 1
		AND CASE WHEN @OnlyShowActive = 1 
				 THEN 
					CASE WHEN E.EndDateTime > GETUTCDATE() THEN 1 ELSE 0 END
				 ELSE 
				 	CASE WHEN E.EndDateTime <= GETUTCDATE() THEN 1 ELSE 0 END
				 END = 1
		AND T.EventId IS NULL -- Not already in the list
	END

	IF (@CreatedByUserId IS NULL AND @ParticipatingInUserId IS NULL)
	BEGIN
		INSERT INTO @tmp
		SELECT E.Id
		FROM dbo.[Events] AS E
		WHERE CASE WHEN @OnlyShowActive = 1 
				 THEN 
					CASE WHEN E.EndDateTime > GETUTCDATE() THEN 1 ELSE 0 END
				 ELSE 
				 	CASE WHEN E.EndDateTime <= GETUTCDATE() THEN 1 ELSE 0 END
				 END = 1
	END

	SELECT e.Id AS EventId
		, E.EventName
		, E.EventDescription
		, E.StartDateTime
		, E.EndDateTime
		, P.DisplayName AS CreatedBy
		, E.Fixtures
		, COUNT(EP.PlayerId) AS NbrPlayers
	FROM @tmp AS T
	INNER JOIN dbo.[Events] AS E ON E.Id = T.EventId
	INNER JOIN Players AS P ON P.Id = E.CreatedByPlayerId
	LEFT OUTER JOIN EventPlayers AS EP ON EP.EventId = E.Id AND EP.[Enabled] = 1
	GROUP BY e.Id
		, E.EventName
		, e.EventDescription
		, E.StartDateTime
		, E.EndDateTime
		, P.DisplayName
		, E.Fixtures

END
GO 