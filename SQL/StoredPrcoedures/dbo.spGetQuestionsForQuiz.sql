USE predictioncomp
GO
-- ================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetQuestionsForQuiz')
DROP PROCEDURE dbo.spGetQuestionsForQuiz
GO

-- =============================================
-- Author:		Pete Salsbury
-- Create date: 6 Apr 2022
-- Description:	Get questions for the quiz
-- =============================================
-- EXEC predictioncomp.dbo.spGetQuestionsForQuiz
CREATE PROCEDURE dbo.spGetQuestionsForQuiz
(
	@strPlayerId NVARCHAR(128) = NULL
	, @intNbrQuestions INT = 5
)
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	SET NOCOUNT ON;

	SELECT TOP (@intNbrQuestions) 
		QQ.Id
		, QQ.PlayerId
		, PL.DisplayName AS QuestionSubmittedBy
		, QQ.QuestionText
		, QQ.AnswerTypeId
	from [dbo].[QuizQuestions] AS QQ
	INNER JOIN Players AS PL ON PL.Id = QQ.PlayerId
	LEFT OUTER JOIN [dbo].[QuizQuestionPlayerAnswers] AS QQPA ON QQPA.QuizQuestionId = QQ.Id AND QQPA.PlayerId = @strPlayerID
	ORDER BY CASE WHEN QQPA.QuizQuestionId IS NULL THEN 0 ELSE 1 END, NEWID()

END
GO