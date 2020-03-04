USE Predict

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
SELECT 'Turkey', 'Italy', '12 June 2020 20:00:00'
UNION ALL SELECT 'Wales', 'Switzerland' ,'13 June 2020 14:00:00'
UNION ALL SELECT 'Denmark', 'Finland' ,'13 June 2020 17:00:00'
UNION ALL SELECT 'Belgium', 'Russia' ,'13 June 2020 20:00:00'
UNION ALL SELECT 'England', 'Croatia' ,'14 June 2020 14:00:00'
UNION ALL SELECT 'Austria', 'PlayOffWinnerD' ,'14 June 2020 17:00:00'
UNION ALL SELECT 'Netherlands ', 'Ukraine' ,'14 June 2020 20:00:00'
UNION ALL SELECT 'PlayOffWinnerC', 'Czech Republic' ,'15 June 2020 14:00:00'
UNION ALL SELECT 'Poland ', 'PlayOffWinnerD' ,'15 June 2020 17:00:00'
UNION ALL SELECT 'Spain', 'Sweden' ,'15 June 2020 20:00:00'
UNION ALL SELECT 'PlayOffWinnerA', 'Portugal' ,'16 June 2020 17:00:00'
UNION ALL SELECT 'France', 'Germany' ,'16 June 2020 20:00:00'
UNION ALL SELECT 'Finland', 'Russia', '17 June 2020 14:00:00'
UNION ALL SELECT 'Turkey', 'Wales' ,'17 June 2020 17:00:00'
UNION ALL SELECT 'Italy', 'Switzerland' ,'17 June 2020 20:00:00'
UNION ALL SELECT 'Ukraine', 'PlayOffWinnerD' ,'18 June 2020 14:00:00'
UNION ALL SELECT 'Denmark', 'Belgium' ,'18 June 2020 17:00:00'
UNION ALL SELECT 'Netherlands', 'Austria' ,'18 June 2020 20:00:00'
UNION ALL SELECT 'Sweden', 'PlayOffWinnerD' ,'19 June 2020 14:00:00'
UNION ALL SELECT 'Croatia', 'Czech Republic' ,'19 June 2020 17:00:00'
UNION ALL SELECT 'England ', 'PlayOffWinnerC' ,'19 June 2020 20:00:00'
UNION ALL SELECT 'PlayOffWinnerA', 'France' ,'20 June 2020 14:00:00'
UNION ALL SELECT 'Portugal', 'Germany' ,'20 June 2020 17:00:00'
UNION ALL SELECT 'Spain', 'Poland' ,'20 June 2020 20:00:00'
UNION ALL SELECT 'Switzerland ', 'Turkey' ,'21 June 2020 17:00:00'
UNION ALL SELECT 'Italy', 'Wales' ,'21 June 2020 17:00:00'
UNION ALL SELECT 'PlayOffWinnerD', 'Netherlands' ,'22 June 2020 17:00:00'
UNION ALL SELECT 'Ukraine ', 'Austria' ,'22 June 2020 17:00:00'
UNION ALL SELECT 'Russia', 'Denmark' ,'22 June 2020 20:00:00'
UNION ALL SELECT 'Finland ', 'Belgium' ,'22 June 2020 20:00:00'
UNION ALL SELECT 'Croatia ', 'PlayOffWinnerC' ,'23 June 2020 20:00:00'
UNION ALL SELECT 'Czech Republic', 'England' ,'23 June 2020 20:00:00'
UNION ALL SELECT 'PlayOffWinnerB', 'Spain' ,'24 June 2020 17:00:00'
UNION ALL SELECT 'Sweden', 'Poland' ,'24 June 2020 17:00:00'
UNION ALL SELECT 'Portugal', 'France' ,'24 June 2020 20:00:00'
UNION ALL SELECT 'Germany', 'PlayOffWinnerA' ,'24 June 2020 20:00:00'

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


SELECT *
FROM [dbo].[Fixtures]