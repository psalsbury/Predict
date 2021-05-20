
DECLARE @intEventId INT = 33

SELECT * FROM Events where id = @intEventId;
SELECT * FROM EventPlayers where EventId = @intEventId;
SELECT * FROM EventFixtures where EventId = @intEventId;
SELECT * FROM EventPools WHERE EventId = @intEventId;
SELECT * FROM EventPoolPlayers WHERE EventId = @intEventId;
SELECT * FROM FixturePredictions WHERE EventId = @intEventId;

BEGIN TRAN

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

rollback TRAN