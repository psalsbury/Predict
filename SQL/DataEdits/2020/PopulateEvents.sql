USE predictioncomp
GO

IF (SELECT COUNT(1) FROM dbo.Events WHERE EventName = 'Euro 2021') = 0
BEGIN

	INSERT INTO dbo.Events
	(
		[EventName]
		, [EventStartDateTime]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT 'Euro 2021', '11 June 2021 20:00:00', GETDATE(), GETDATE()

END;