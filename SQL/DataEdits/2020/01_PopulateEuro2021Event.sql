USE predictioncomp
GO

DECLARE @intDefaultPoolId INT

SELECT @intDefaultPoolId = Id
FROM dbo.Pools WITH (NOLOCK)
WHERE PoolName = 'Global Pool'

IF (SELECT COUNT(1) FROM dbo.Events WHERE EventName = 'Euro 2021') = 0
BEGIN

	INSERT INTO dbo.Events
	(
		[EventName]
		, [EventDescription]
		, [CreatedDateTime]
		, [ModifiedDateTime]
		, [StartDateTime]
		, [EndDateTime]
		, [DefaultPoolId]
	)
	SELECT 'Euro 2021'
			, 'EUFA Euro 2021 Tournament'
			, GETDATE()
			, GETDATE()
			, '11 June 2021 20:00:00'
			, '11 July 2021 20:00:00'
			, @intDefaultPoolId
END;

