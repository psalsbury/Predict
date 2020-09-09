USE predictioncomp
GO

DECLARE @intEventId INT

SELECT @intEventId = Id
FROM dbo.Events
WHERE EventName = 'Euro 2021'

IF (SELECT COUNT(1) FROM dbo.BonusQuestions WHERE EventId = @intEventId) = 0
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
	SELECT @intEventId
			, 'Time of the first goal in the Euro 2021 Final'
			, 6
			, '11 July 2021 20:00'
			, GETDATE()
			, GETDATE()
	UNION ALL
	SELECT @intEventId
			, 'Total number of goals in both semi finals'
			, 6
			, '6 July 2021 20:00'
			, GETDATE()
			, GETDATE()

END;


