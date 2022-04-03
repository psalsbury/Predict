USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetJokesLeagues')
DROP PROCEDURE dbo.spGetJokesLeagues
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 01 Apr 2022
-- Description:	Get the league table for jokes
-- =============================================
-- EXEC predictioncomp.dbo.spGetJokesLeagues
CREATE PROCEDURE dbo.spGetJokesLeagues
(
	@strPlayerId NVARCHAR(256) = NULL
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT ROW_NUMBER() OVER(ORDER BY AVG(CAST(JR.PlayerJokeRating AS DECIMAL)) DESC, COUNT(JR.PlayerJokeRating) DESC) AS Position
		, JO.ID
		, JO.JokeText
		, ISNULL(AVG(CAST(JR.PlayerJokeRating AS DECIMAL)),0) AS AvgRating
		, COUNT(JR.PlayerJokeRating) AS NbrRatings
	FROM Jokes AS JO
	LEFT OUTER JOIN JokeRatings AS JR ON JR.JokeId = JO.Id
	WHERE JO.PlayerId = CASE WHEN @strPlayerId IS NULL THEN JO.PlayerId ELSE @strPlayerId END
	GROUP BY JO.ID
		, JO.JokeText
	ORDER BY AVG(CAST(JR.PlayerJokeRating AS DECIMAL)) DESC, COUNT(JR.PlayerJokeRating) DESC

END
GO