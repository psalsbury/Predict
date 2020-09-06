use predictioncomp

DECLARE @intEventId INT

SELECT @intEventId = Id
FROM dbo.Events
WHERE EventName = 'Euro 2021'

INSERT INTO [dbo].[KoFixtures]
(
	EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, Team1FromLeagueId
	, Team1FromLeaguePosition
	, Team2FromLeagueId
	, Team2FromLeaguePosition
	, CreatedDateTime
	, ModifiedDateTime
)
SELECT @intEventId, '26 June 2021 17:00:00'           ,16,1  ,'1' ,2 ,'2' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '26 June 2021 20:00:00' ,16,2  ,'1' ,1 ,'3' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '27 June 2021 17:00:00' ,16,3  ,'3' ,1 ,'4,5,6' ,3 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '27 June 2021 20:00:00' ,16,4  ,'2' ,1 ,'1,4,5,6' ,3 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '28 June 2021 17:00:00' ,16,5  ,'4' ,2 ,'5' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '28 June 2021 20:00:00' ,16,6  ,'6' ,1 ,'1,2,3' ,3 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '29 June 2021 17:00:00' ,16,7  ,'4' ,1 ,'6' ,2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '29 June 2021 20:00:00' ,16,8  ,'5' ,1 ,'1,2,3,4' ,3 ,GETDATE() ,GETDATE()

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
SELECT @intEventId, '2 July 2021 17:00:00'			 ,8,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '2 July 2021 20:00:00' ,8,2,  @intMinId+3,@intMinId+4 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '3 July 2021 17:00:00' ,8,3,  @intMinId+5,@intMinId+6 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '3 July 2021 20:00:00' ,8,4,  @intMinId+7,@intMinId+8 ,GETDATE() ,GETDATE()

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
SELECT @intEventId, '6 July 2021 20:00:00' ,4,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '7 July 2021 20:00:00' ,4,2,  @intMinId+3,@intMinId+4 ,GETDATE() ,GETDATE()

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
SELECT @intEventId, '11 July 2021 20:00:00' ,2,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()

