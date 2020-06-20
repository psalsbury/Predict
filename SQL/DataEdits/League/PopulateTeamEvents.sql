USE Predict

DECLARE @intEventId INT = 2

CREATE TABLE #tmp
(TeamName varchar(50)
,League varchar(50)
)
INSERT INTO #tmp
SELECT 'Arsenal','Premier League'
UNION ALL SELECT 'Aston Villa','Premier League'
UNION ALL SELECT 'Bournemouth','Premier League'
UNION ALL SELECT 'Brighton','Premier League'
UNION ALL SELECT 'Burnley','Premier League'
UNION ALL SELECT 'Chelsea','Premier League'
UNION ALL SELECT 'Crsytal Palace','Premier League'
UNION ALL SELECT 'Everton','Premier League'
UNION ALL SELECT 'Leicester City','Premier League'
UNION ALL SELECT 'Liverpool','Premier League'
UNION ALL SELECT 'Manchester City','Premier League'
UNION ALL SELECT 'Manchester United','Premier League'
UNION ALL SELECT 'Newcastle United','Premier League'
UNION ALL SELECT 'Norwich City','Premier League'
UNION ALL SELECT 'Sheffield United','Premier League'
UNION ALL SELECT 'Southampton','Premier League'
UNION ALL SELECT 'Tottenham Hotspur','Premier League'
UNION ALL SELECT 'Watford','Premier League'
UNION ALL SELECT 'West Ham','Premier League'
UNION ALL SELECT 'Wolves','Premier League'


IF (SELECT COUNT(1) FROM [dbo].[EventTeams] WHERE EventId=@intEventId) = 0
BEGIN

	INSERT INTO [dbo].[EventTeams]
	(TeamId
	, EventId
	, League
	, [CreatedDateTime]
	, [ModifiedDateTime]
	)
	SELECT T.Id
		, @intEventId
		, TMP.League
		, GETDATE()
		, GETDATE()
	FROM #tmp AS TMP
	INNER JOIN [dbo].[Teams] AS T ON T.TeamName = TMP.TeamName
	LEFT OUTER JOIN [dbo].[EventTeams] ET ON ET.TeamId = T.Id AND ET.EventId = @intEventId
	WHERE ET.TeamId IS NULL
END;
