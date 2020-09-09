use predictioncomp

DECLARE @intEventId INT

SELECT @intEventId = Id
FROM dbo.Events
WHERE EventName = 'Euro 2021'

CREATE TABLE #TMP
(
	FixtureDateTime DATETIME
	, RoundOf INT
	, Position INT
	, Team1FromLeaguePosition INT
	, Team1FromLeagueShortName VARCHAR(50)
	, Team2FromLeaguePosition INT
	, Team2FromLeagueShortName VARCHAR(50)
)

INSERT INTO #TMP
(
	FixtureDateTime
	, RoundOf
	, Position
	, Team1FromLeaguePosition
	, Team1FromLeagueShortName
	, Team2FromLeaguePosition
	, Team2FromLeagueShortName
)
SELECT '27 June 2021 00:00:00'           ,16,1		,1 ,'B'		,3 ,'' 
UNION ALL SELECT '26 June 2021 00:00:00' ,16,2		,1 ,'A'		,2 ,'C'
UNION ALL SELECT '28 June 2021 00:00:00' ,16,3		,1 ,'F'		,3 ,'' 
UNION ALL SELECT '28 June 2021 00:00:00' ,16,4		,2 ,'D'		,2 ,'E'
UNION ALL SELECT '29 June 2021 00:00:00' ,16,5		,1 ,'E'		,3 ,'' 
UNION ALL SELECT '29 June 2021 00:00:00' ,16,6		,1 ,'D'		,2 ,'F'
UNION ALL SELECT '27 June 2021 00:00:00' ,16,7		,1 ,'C'		,3 ,'' 
UNION ALL SELECT '26 June 2021 00:00:00' ,16,8		,2 ,'A'		,2 ,'B'

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
SELECT @intEventId
	,FixtureDateTime
	, RoundOf
	, Position
	, L1.Id
	, TMP.Team1FromLeaguePosition
	, ISNULL(L2.Id,0)
	, TMP.Team2FromLeaguePosition
	,GETDATE() 
	,GETDATE()
FROM #TMP TMP
INNER JOIN [dbo].[Leagues] AS L1 ON L1.ShortLeagueName = TMP.Team1FromLeagueShortName AND L1.LeagueName LIKE '%EURO%2021%'
LEFT OUTER JOIN [dbo].[Leagues] AS L2 ON L2.ShortLeagueName = TMP.Team2FromLeagueShortName AND L2.LeagueName LIKE '%EURO%2021%'

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
SELECT @intEventId, '2 July 2021 00:00:00'			 ,8,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '2 July 2021 00:00:00' ,8,2,  @intMinId+3,@intMinId+4 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '3 July 2021 00:00:00' ,8,3,  @intMinId+5,@intMinId+6 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '3 July 2021 00:00:00' ,8,4,  @intMinId+7,@intMinId+8 ,GETDATE() ,GETDATE()

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
SELECT @intEventId, '6 July 2021 00:00:00' ,4,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()
UNION ALL SELECT @intEventId, '7 July 2021 00:00:00' ,4,2,  @intMinId+3,@intMinId+4 ,GETDATE() ,GETDATE()

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
SELECT @intEventId, '11 July 2021 00:00:00' ,2,1,  @intMinId+1,@intMinId+2 ,GETDATE() ,GETDATE()

