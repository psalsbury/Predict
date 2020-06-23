USE Predictioncomp
GO

IF (SELECT COUNT(1) FROM dbo.Events WHERE EventName = 'Prediction Round 1') = 0
BEGIN

	INSERT INTO dbo.Events
	(
		[EventName]
		, [EventStartDateTime]
		, [CreatedDateTime]
		, [ModifiedDateTime]
		, DefaultPoolId
	)
	SELECT 'Prediction Round 1', '27 June 2020 11:30:00', GETDATE(), GETDATE(), 0

END;
