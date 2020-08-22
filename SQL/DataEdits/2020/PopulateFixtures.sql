USE predictioncomp

DECLARE @intEventId INT = 2

DELETE FROM Fixtures
WHERE EventId = @intEventId

CREATE TABLE #tmp
(
	TeamName1 varchar(50)
	,TeamName2 varchar(50)
	,[FixtureDateTime] datetime
)
INSERT INTO #tmp
SELECT 'Turkey', 'Italy', '11 June 2021 20:00:00'
UNION ALL SELECT 'Wales', 'Switzerland' ,'12 June 2021 14:00:00'
UNION ALL SELECT 'Denmark', 'Finland' ,'12 June 2021 17:00:00'
UNION ALL SELECT 'Belgium', 'Russia' ,'12 June 2021 20:00:00'
UNION ALL SELECT 'England', 'Croatia' ,'13 June 2021 14:00:00'
UNION ALL SELECT 'Austria', 'PlayOffWinnerD' ,'13 June 2021 17:00:00'
UNION ALL SELECT 'Netherlands ', 'Ukraine' ,'13 June 2021 20:00:00'
UNION ALL SELECT 'PlayOffWinnerC', 'Czech Republic' ,'14 June 2021 14:00:00'
UNION ALL SELECT 'Poland ', 'PlayOffWinnerD' ,'14 June 2021 17:00:00'
UNION ALL SELECT 'Spain', 'Sweden' ,'14 June 2021 20:00:00'
UNION ALL SELECT 'PlayOffWinnerA', 'Portugal' ,'15 June 2021 17:00:00'
UNION ALL SELECT 'France', 'Germany' ,'15 June 2021 20:00:00'
UNION ALL SELECT 'Finland', 'Russia', '16 June 2021 14:00:00'
UNION ALL SELECT 'Turkey', 'Wales' ,'16 June 2021 17:00:00'
UNION ALL SELECT 'Italy', 'Switzerland' ,'16 June 2021 20:00:00'
UNION ALL SELECT 'Ukraine', 'PlayOffWinnerD' ,'17 June 2021 14:00:00'
UNION ALL SELECT 'Denmark', 'Belgium' ,'17 June 2021 17:00:00'
UNION ALL SELECT 'Netherlands', 'Austria' ,'17 June 2021 20:00:00'
UNION ALL SELECT 'Sweden', 'PlayOffWinnerD' ,'18 June 2021 14:00:00'
UNION ALL SELECT 'Croatia', 'Czech Republic' ,'18 June 2021 17:00:00'
UNION ALL SELECT 'England ', 'PlayOffWinnerC' ,'18 June 2021 20:00:00'
UNION ALL SELECT 'PlayOffWinnerA', 'France' ,'19 June 2021 14:00:00'
UNION ALL SELECT 'Portugal', 'Germany' ,'19 June 2021 17:00:00'
UNION ALL SELECT 'Spain', 'Poland' ,'19 June 2021 20:00:00'
UNION ALL SELECT 'Switzerland ', 'Turkey' ,'20 June 2021 17:00:00'
UNION ALL SELECT 'Italy', 'Wales' ,'20 June 2021 17:00:00'
UNION ALL SELECT 'PlayOffWinnerD', 'Netherlands' ,'20 June 2021 17:00:00'
UNION ALL SELECT 'Ukraine ', 'Austria' ,'21 June 2021 17:00:00'
UNION ALL SELECT 'Russia', 'Denmark' ,'21 June 2021 20:00:00'
UNION ALL SELECT 'Finland ', 'Belgium' ,'21 June 2021 20:00:00'
UNION ALL SELECT 'Croatia ', 'PlayOffWinnerC' ,'22 June 2021 20:00:00'
UNION ALL SELECT 'Czech Republic', 'England' ,'22 June 2021 20:00:00'
UNION ALL SELECT 'PlayOffWinnerB', 'Spain' ,'23 June 2021 17:00:00'
UNION ALL SELECT 'Sweden', 'Poland' ,'23 June 2021 17:00:00'
UNION ALL SELECT 'Portugal', 'France' ,'23 June 2021 20:00:00'
UNION ALL SELECT 'Germany', 'PlayOffWinnerA' ,'23 June 2021 20:00:00'


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


