USE predictioncomp

DECLARE @intEventId INT = 2

CREATE TABLE #tmp
(TeamName varchar(50)
,League varchar(50)
)
INSERT INTO #tmp
SELECT 'Turkey', 'A'
UNION ALL SELECT 'Italy', 'A'
UNION ALL SELECT 'Wales', 'A'
UNION ALL SELECT 'Switzerland', 'A'
UNION ALL SELECT 'Denmark', 'B'
UNION ALL SELECT 'Finland', 'B'
UNION ALL SELECT 'Belgium', 'B'
UNION ALL SELECT 'Russia', 'B'
UNION ALL SELECT 'Netherlands', 'C'
UNION ALL SELECT 'Ukraine', 'C'
UNION ALL SELECT 'Austria', 'C'
UNION ALL SELECT 'PlayOffD', 'C'
UNION ALL SELECT 'England', 'D'
UNION ALL SELECT 'Croatia', 'D'
UNION ALL SELECT 'PlayOffC', 'D'
UNION ALL SELECT 'Czech Republic', 'D'
UNION ALL SELECT 'Spain', 'E'
UNION ALL SELECT 'Sweden', 'E'
UNION ALL SELECT 'Poland', 'E'
UNION ALL SELECT 'PlayOffB', 'E'
UNION ALL SELECT 'PlayOffA', 'F'
UNION ALL SELECT 'Portugal', 'F'
UNION ALL SELECT 'France', 'F'
UNION ALL SELECT 'Germany', 'F'




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
