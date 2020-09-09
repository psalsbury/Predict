USE predictioncomp

CREATE TABLE #tmp
(
	TeamName1 varchar(50)
	,TeamName2 varchar(50)
	,[FixtureDateTime] datetime
	,LeagueId INT
)
INSERT INTO #tmp
(
	TeamName1
	,TeamName2
	,[FixtureDateTime] 
	,LeagueId 
)
SELECT 'Turkey', 'Italy', '11 June 2021 20:00:00', 1
UNION ALL SELECT 'Wales', 'Switzerland' ,'12 June 2021 14:00:00', 1
UNION ALL SELECT 'Denmark', 'Finland' ,'12 June 2021 17:00:00', 2
UNION ALL SELECT 'Belgium', 'Russia' ,'12 June 2021 20:00:00', 2
UNION ALL SELECT 'England', 'Croatia' ,'13 June 2021 14:00:00', 4
UNION ALL SELECT 'Austria', 'PlayOffC' ,'13 June 2021 17:00:00', 3
UNION ALL SELECT 'Netherlands ', 'Ukraine' ,'13 June 2021 20:00:00', 3
UNION ALL SELECT 'PlayOffD', 'Czech Republic' ,'14 June 2021 14:00:00', 4
UNION ALL SELECT 'Poland ', 'PlayOffE' ,'14 June 2021 17:00:00', 5
UNION ALL SELECT 'Spain', 'Sweden' ,'14 June 2021 20:00:00', 5
UNION ALL SELECT 'PlayOffF', 'Portugal' ,'15 June 2021 17:00:00', 6
UNION ALL SELECT 'France', 'Germany' ,'15 June 2021 20:00:00', 6
UNION ALL SELECT 'Finland', 'Russia', '16 June 2021 14:00:00', 2
UNION ALL SELECT 'Turkey', 'Wales' ,'16 June 2021 17:00:00', 1
UNION ALL SELECT 'Italy', 'Switzerland' ,'16 June 2021 20:00:00', 1
UNION ALL SELECT 'Ukraine', 'PlayOffC' ,'17 June 2021 14:00:00', 3
UNION ALL SELECT 'Denmark', 'Belgium' ,'17 June 2021 17:00:00', 2
UNION ALL SELECT 'Netherlands', 'Austria' ,'17 June 2021 20:00:00', 3
UNION ALL SELECT 'Sweden', 'PlayOffE' ,'18 June 2021 14:00:00', 5
UNION ALL SELECT 'Croatia', 'Czech Republic' ,'18 June 2021 17:00:00', 4
UNION ALL SELECT 'England ', 'PlayOffD' ,'18 June 2021 20:00:00', 4
UNION ALL SELECT 'PlayOffF', 'France' ,'19 June 2021 14:00:00', 6
UNION ALL SELECT 'Portugal', 'Germany' ,'19 June 2021 17:00:00', 6
UNION ALL SELECT 'Spain', 'Poland' ,'19 June 2021 20:00:00', 5
UNION ALL SELECT 'Switzerland ', 'Turkey' ,'20 June 2021 17:00:00', 1
UNION ALL SELECT 'Italy', 'Wales' ,'20 June 2021 17:00:00', 1
UNION ALL SELECT 'PlayOffC', 'Netherlands' ,'20 June 2021 17:00:00', 3
UNION ALL SELECT 'Ukraine ', 'Austria' ,'21 June 2021 17:00:00', 3
UNION ALL SELECT 'Russia', 'Denmark' ,'21 June 2021 20:00:00', 2
UNION ALL SELECT 'Finland ', 'Belgium' ,'21 June 2021 20:00:00', 2
UNION ALL SELECT 'Croatia ', 'PlayOffD' ,'22 June 2021 20:00:00', 4
UNION ALL SELECT 'Czech Republic', 'England' ,'22 June 2021 20:00:00', 4
UNION ALL SELECT 'PlayOffE', 'Spain' ,'23 June 2021 17:00:00', 5
UNION ALL SELECT 'Sweden', 'Poland' ,'23 June 2021 17:00:00', 5
UNION ALL SELECT 'Portugal', 'France' ,'23 June 2021 20:00:00', 6
UNION ALL SELECT 'Germany', 'PlayOffF' ,'23 June 2021 20:00:00', 6

INSERT INTO [dbo].[Fixtures]
(
	 [FixtureDateTime]
	, [HomeTeamId]
	, [AwayTeamId]
	, [CreatedDateTime]
	, [ModifiedDateTime]
	, LeagueId
)
SELECT TMP.FixtureDateTime
	, TH.Id
	, TA.Id
	, GETDATE()
	, GETDATE()
	, TMP.LeagueId
FROM #tmp AS TMP
INNER JOIN [dbo].[Teams] AS TH ON TH.TeamName = TMP.TeamName1
INNER JOIN [dbo].[Teams] AS TA ON TA.TeamName = TMP.TeamName2
LEFT OUTER JOIN [dbo].[Fixtures] F ON F.[HomeTeamId] = TH.Id AND F.AwayTeamId = TH.Id AND F.LeagueId = tmp.LeagueId
WHERE F.Id IS NULL

