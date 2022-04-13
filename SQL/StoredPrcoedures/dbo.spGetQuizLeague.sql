USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetQuizLeague')
DROP PROCEDURE dbo.spGetQuizLeague
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 13 Apr 2022
-- Description:	Get the league table for quiz
-- =============================================
-- EXEC predictioncomp.dbo.spGetQuizLeague
CREATE PROCEDURE dbo.spGetQuizLeague
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	-- Get the Quiz table
	SELECT PL.DisplayName AS PlayerName
		, TE.TeamFlag
		, COUNT(QQPA.QuizQuestionId) as QuestionsAnswered
		, SUM(CASE WHEN QQPA.IsCorrect=1 THEN 1 ELSE 0 END) as QuestionsCorrect
		, (SELECT COUNT(1) FROM dbo.QuizQuestions QQ WHERE QQ.PlayerId <> QQPA.PlayerId) AS QuestionsAvailable
		, CASE WHEN 
						(SELECT COUNT(1) FROM dbo.QuizQuestions QQ WHERE QQ.PlayerId <> QQPA.PlayerId) = 0 
						OR 
						COUNT(QQPA.QuizQuestionId) = 0 THEN 0
				ELSE
					(
						CAST(COUNT(QQPA.QuizQuestionId) as DECIMAL)
							/ 
						CAST((SELECT COUNT(1) FROM dbo.QuizQuestions QQ WHERE QQ.PlayerId <> QQPA.PlayerId) as DECIMAL) 
					)
					*
					(
						CAST(SUM(CASE WHEN QQPA.IsCorrect=1 THEN 1 ELSE 0 END) as DECIMAL) 
							/ 
						CAST(COUNT(QQPA.QuizQuestionId) as DECIMAL)
					)
			END AS Score
	FROM dbo.QuizQuestionPlayerAnswers AS QQPA
	INNER JOIN dbo.QuizQuestions AS QUQ ON QUQ.Id = QQPA.QuizQuestionId
	INNER JOIN dbo.Players AS PL ON PL.Id = QQPA.PlayerId
	LEFT OUTER JOIN dbo.Teams AS TE ON TE.Id = PL.SupportTeamId
	WHERE QQPA.PlayerId <> QUQ.PlayerId
	GROUP BY QQPA.PlayerId
			, PL.Displayname
			, TE.TeamFlag
			, TE.TeamName
	ORDER BY Score
END
GO
