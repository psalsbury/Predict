USE predictioncomp

DECLARE @intEventId INT

SELECT @intEventId = Id
FROM dbo.Events
WHERE EventName = 'Euro 2021'

IF (SELECT COUNT(1) FROM [dbo].[EventFixtures] WHERE EventId = @intEventId) = 0
BEGIN

	INSERT INTO [dbo].[EventFixtures]
	(
		EventId
		, [FixtureId]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT @intEventId
		, F.Id
		, GETDATE()
		, GETDATE()
	FROM [dbo].[Fixtures] AS F
	WHERE F.LeagueId IN (1,2,3,4,5,6)

END