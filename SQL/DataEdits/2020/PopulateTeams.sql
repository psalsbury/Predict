USE predictioncomp
GO

CREATE TABLE #tmp
(
	[TeamName]  VARCHAR(50)
	, [TeamFlag] VARCHAR(100)
)

INSERT INTO #tmp
(
	[TeamName]
	,[TeamFlag]
)


SELECT 'Austria','Austria.gif'
UNION ALL SELECT 'Belgium','Belgium.gif'
UNION ALL SELECT 'Croatia','Croatia.gif'
UNION ALL SELECT 'Czech Republic','CzechRepublic.gif'
UNION ALL SELECT 'Denmark','Denmark.gif'
UNION ALL SELECT 'England','England.gif'
UNION ALL SELECT 'Finland','Finland.png'
UNION ALL SELECT 'France','France.gif'
UNION ALL SELECT 'Germany','Germany.gif'
UNION ALL SELECT 'Italy','Italy.gif'
UNION ALL SELECT 'Netherlands','Netherlands.gif'
UNION ALL SELECT 'Poland','Poland.gif'
UNION ALL SELECT 'Portugal','Portugal.gif'
UNION ALL SELECT 'Russia','Russia.gif'
UNION ALL SELECT 'Spain','Spain.gif'
UNION ALL SELECT 'Sweden','Sweden.gif'
UNION ALL SELECT 'Switzerland','Switzerland.gif'
UNION ALL SELECT 'Turkey','Turkey.gif'
UNION ALL SELECT 'Ukraine','Ukraine.gif'
UNION ALL SELECT 'Wales','Wales.gif'
UNION ALL SELECT 'PlayOffA','PlayOffA.gif'
UNION ALL SELECT 'PlayOffB','PlayOffB.gif'	
UNION ALL SELECT 'PlayOffC','PlayOffC.gif'
UNION ALL SELECT 'PlayOffD','PlayOffD.gif'

INSERT INTO dbo.Teams
(
	[TeamName]
	, [TeamFlag]
	, [CreatedDateTime]
	, [ModifiedDateTime]
)
SELECT TMP.TeamName
	, TMP.TeamFlag
	, GETDATE()
	, GETDATE()
FROM #tmp AS TMP
LEFT OUTER JOIN dbo.Teams AS T ON T.TeamName = TMP.TeamName
WHERE T.TeamName IS NULL
	
