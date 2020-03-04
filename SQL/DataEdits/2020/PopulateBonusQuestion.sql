USE Predict
GO

IF (SELECT COUNT(1) FROM dbo.BonusQuestions) = 0
BEGIN

	INSERT INTO dbo.BonusQuestions
	(
		EventId
		, [Question]
		, [Score]
		, [ToBeAnsweredByDateTime]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT 1
			, 'Time of the first goal in the Euro 2020 Final'
			, 6
			, '12 July 2020 20:00'
			, GETDATE()
			, GETDATE()
	UNION ALL
	SELECT 1
			, 'Total number of goals in both semi finals'
			, 6
			, '7 July 2020 20:00'
			, GETDATE()
			, GETDATE()

END;