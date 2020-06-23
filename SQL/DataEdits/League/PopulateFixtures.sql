USE Predictioncomp

DECLARE @intEventId INT = 1

DELETE FROM Fixtures
WHERE EventId = @intEventId

CREATE TABLE #tmp
(
	TeamName1 varchar(50)
	,TeamName2 varchar(50)
	,[FixtureDateTime] datetime
)
INSERT INTO #tmp
SELECT 'Aston Villa','Wolves','27 Jun 20 12:30:00'
UNION ALL SELECT 'Watford','Southampton','28 Jun 20 16:30:00'
UNION ALL SELECT 'Crystal Palace','Burnley','29 Jun 20 20:00:00'
UNION ALL SELECT 'Brighton','Manchester United','30 Jun 20 20:15:00'
UNION ALL SELECT 'Arsenal','Norwich City','01 Jul 20 18:00:00'
UNION ALL SELECT 'Bournemouth','Newcastle United','01 Jul 20 18:00:00'
UNION ALL SELECT 'Everton','Leicester City','01 Jul 20 18:00:00'
UNION ALL SELECT 'West Ham','Chelsea','01 Jul 20 20:15:00'
UNION ALL SELECT 'Sheffield United','Tottenham Hotspur','02 Jul 20 18:00:00'
UNION ALL SELECT 'Manchester City','Liverpool','02 Jul 20 20:15:00'
UNION ALL SELECT 'Norwich City','Brighton','04 Jul 20 12:30:00'
UNION ALL SELECT 'Leicester City','Crystal Palace','04 Jul 20 15:00:00'
UNION ALL SELECT 'Manchester United','Bournemouth','04 Jul 20 15:00:00'
UNION ALL SELECT 'Wolves','Arsenal','04 Jul 20 17:30:00'
UNION ALL SELECT 'Chelsea','Watford','04 Jul 20 20:00:00'
UNION ALL SELECT 'Burnley','Sheffield United','05 Jul 20 12:00:00'
UNION ALL SELECT 'Newcastle United','West Ham','05 Jul 20 14:00:00'
UNION ALL SELECT 'Liverpool','Aston Villa','05 Jul 20 16:30:00'
UNION ALL SELECT 'Southampton','Manchester City','05 Jul 20 19:00:00'
UNION ALL SELECT 'Tottenham Hotspur','Everton','06 Jul 20 20:00:00'
UNION ALL SELECT 'Crystal Palace','Chelsea','07 Jul 20 18:00:00'
UNION ALL SELECT 'Watford','Norwich City','07 Jul 20 18:00:00'
UNION ALL SELECT 'Arsenal','Leicester City','07 Jul 20 20:15:00'
UNION ALL SELECT 'Manchester City','Newcastle United','08 Jul 20 18:00:00'
UNION ALL SELECT 'Sheffield United','Wolves','08 Jul 20 18:00:00'
UNION ALL SELECT 'West Ham','Burnley','08 Jul 20 18:00:00'
UNION ALL SELECT 'Brighton','Liverpool','08 Jul 20 20:15:00'
UNION ALL SELECT 'Bournemouth','Tottenham Hotspur','09 Jul 20 18:00:00'
UNION ALL SELECT 'Everton','Southampton','09 Jul 20 18:00:00'
UNION ALL SELECT 'Aston Villa','Manchester United','09 Jul 20 20:15:00'




IF (SELECT COUNT(1) FROM [dbo].[Fixtures] WHERE EventId=@intEventId) = 0
BEGIN

	INSERT INTO [dbo].[Fixtures]
	(
		[EventId]
		, [FixtureDateTime]
		, [HomeTeamId]
		, [AwayTeamId]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT @intEventId
		, TMP.FixtureDateTime
		, TH.Id
		, TA.Id
		, GETDATE()
		, GETDATE()
	FROM #tmp AS TMP
	INNER JOIN [dbo].[Teams] AS TH ON TH.TeamName = TMP.TeamName1
	INNER JOIN [dbo].[Teams] AS TA ON TA.TeamName = TMP.TeamName2
	LEFT OUTER JOIN [dbo].[Fixtures] F ON F.[HomeTeamId] = TH.Id AND F.AwayTeamId = TH.Id AND F.EventId = @intEventId
	WHERE F.Id IS NULL
END;


UPDATE Fixtures
SET [FixtureDateTime] = DATEADD(HOUR,-1,[FixtureDateTime])
WHERE EventId = 1;