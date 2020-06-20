USE Predict
GO

IF (SELECT COUNT(1) FROM dbo.Events WHERE EventName = 'Think Social Comp 1') = 0
BEGIN

	INSERT INTO Predict.dbo.Events
	(
		[EventName]
		, [EventStartDateTime]
		, [PlayerDeadlineDateTime]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT 'Think Social Comp 1', '27 June 2020 11:30:00', '27 June 2020 11:30:00', GETDATE(), GETDATE()

END;
