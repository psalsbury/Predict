

CREATE TABLE #tmp
(Pos INT IDENTITY(1,1)
,EventId INT)

INSERT INTO #TMP
SELECT 37
UNION ALL SELECT 38
UNION ALL SELECT 42
UNION ALL SELECT 46
UNION ALL SELECT 47
UNION ALL SELECT 48
UNION ALL SELECT 49
UNION ALL SELECT 51
UNION ALL SELECT 52
UNION ALL SELECT 53
UNION ALL SELECT 58

DECLARE @intEventId INT = 0
DECLARE @max INT
DECLARE @int INT = 1

SELECT @max = MAX(Pos) FROM #TMP

WHILE @int <= @max
BEGIN

	SELECT @intEventId = EVENTID FROM #TMP WHERE POS = @INT

	SELECT * FROM Events where id = @intEventId;
	SELECT * FROM EventPlayers where EventId = @intEventId;
	SELECT * FROM EventFixtures where EventId = @intEventId;
	SELECT * FROM EventPools WHERE EventId = @intEventId;
	SELECT * FROM EventPoolPlayers WHERE EventId = @intEventId;
	SELECT * FROM FixturePredictions WHERE EventId = @intEventId;

	DELETE Events where id = @intEventId;
	DELETE EventPlayers where EventId = @intEventId;
	DELETE EventFixtures where EventId = @intEventId;
	DELETE EventPools WHERE EventId = @intEventId;
	DELETE EventPoolPlayers WHERE EventId = @intEventId;
	DELETE FixturePredictions WHERE EventId = @intEventId;

	SELECT * FROM Events where id = @intEventId;
	SELECT * FROM EventPlayers where EventId = @intEventId;
	SELECT * FROM EventFixtures where EventId = @intEventId;
	SELECT * FROM EventPools WHERE EventId = @intEventId;
	SELECT * FROM EventPoolPlayers WHERE EventId = @intEventId;
	SELECT * FROM FixturePredictions WHERE EventId = @intEventId;

	SELECT @int=@int+1

END

