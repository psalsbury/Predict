USE predictioncomp
GO

DECLARE @intDefaultPoolId INT
DECLARE @intEventId INT
DECLARE @strCreatedById NVARCHAR(128)

SELECT @intDefaultPoolId = Id
FROM dbo.Pools WITH (NOLOCK)
WHERE PoolName = 'Global Pool'

SELECT  @strCreatedById = Id
FROM [dbo].[AspNetUsers]
WHERE Email = 'pete@salsbury.co.uk'

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
		, CreatedByPlayerId
	)
	SELECT 'Euro 2021'
			, 'EUFA Euro 2021 Tournament'
			, GETDATE()
			, GETDATE()
			, '11 June 2021 20:00:00'
			, '11 July 2021 20:00:00'
			, @intDefaultPoolId
			, @strCreatedById

	SELECT @intEventId = SCOPE_IDENTITY()

	/* Ensure that the Pool and Event are associated */
	INSERT INTO EventPools
	(
		[EventId]
		, [PoolId]
		, [CreatedDateTime]
		, [ModifiedDateTime]
		, Enabled
	)
	SELECT @intEventId
		, @intDefaultPoolId
		, GETDATE()
		, GETDATE()
		, 1
END;

