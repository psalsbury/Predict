DECLARE @intEventId INT = 1

INSERT INTO [dbo].[KoFixtures]
(
	EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, Team1FromLeague
	, Team1FromLeaguePosition
	, Team2FromLeague
	, Team2FromLeaguePosition
	, CreatedDateTime
	, ModifiedDateTime
)
SELECT @intEventId, '27 June 2020 17:00:00'           ,16,1  ,'A' ,2 ,'B' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '27 June 2020 20:00:00' ,16,2  ,'A' ,1 ,'C' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '28 June 2020 17:00:00' ,16,3  ,'C' ,1 ,'DEF' ,3 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '28 June 2020 20:00:00' ,16,4  ,'B' ,1 ,'ADEF' ,3 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '29 June 2020 17:00:00' ,16,5  ,'D' ,2 ,'E' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '29 June 2020 20:00:00' ,16,6  ,'F' ,1 ,'ABC' ,3 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '30 June 2020 17:00:00' ,16,7  ,'D' ,1 ,'F' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '30 June 2020 20:00:00' ,16,8  ,'E' ,1 ,'ABCD' ,3 ,GETDATE() ,GETDATE()

DECLARE @intMinId INT

SELECT @intMinId = MIN(Id)-1
FROM [dbo].[KoFixtures]
WHERE EventId = @intEventId

INSERT INTO [dbo].[KoFixtures]
(
	EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, [Team1FromKoFixtureId]
	, [Team2FromKoFixtureId]
	, CreatedDateTime
	, ModifiedDateTime
)
SELECT @intEventId, '3 July 2020 17:00:00'			 ,8,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '3 July 2020 20:00:00' ,8,2,  @intMinId+3,@intMinId+4 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '4 July 2020 17:00:00' ,8,3,  @intMinId+5,@intMinId+6 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '4 July 2020 20:00:00' ,8,4,  @intMinId+7,@intMinId+8 ,GETDATE() ,GETDATE()

SELECT @intMinId = MIN(Id)-1
FROM [dbo].[KoFixtures]
WHERE EventId = @intEventId
AND RoundOf = 8

INSERT INTO [dbo].[KoFixtures]
(
	EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, [Team1FromKoFixtureId]
	, [Team2FromKoFixtureId]
	, CreatedDateTime
	, ModifiedDateTime
)
SELECT @intEventId, '7 July 2020 20:00:00' ,4,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '8 July 2020 20:00:00' ,4,2,  @intMinId+3,@intMinId+4 ,GETDATE() ,GETDATE()

SELECT @intMinId = MIN(Id)-1
FROM [dbo].[KoFixtures]
WHERE EventId = @intEventId
AND RoundOf = 4

INSERT INTO [dbo].[KoFixtures]
(
	EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, [Team1FromKoFixtureId]
	, [Team2FromKoFixtureId]
	, CreatedDateTime
	, ModifiedDateTime
)
SELECT @intEventId, '12 July 2020 20:00:00' ,2,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()


