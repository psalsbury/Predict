USE Predict
GO

IF (SELECT COUNT(1) FROM dbo.Events) = 0
BEGIN

	INSERT INTO Predict.dbo.Events
	(
		[EventName]
		, [EventStartDateTime]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT 'Euro 2020', '12 June 2020 20:00:00', GETDATE(), GETDATE()

END;