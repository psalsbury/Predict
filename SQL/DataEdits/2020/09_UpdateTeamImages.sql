USE predictioncomp
GO

CREATE TABLE #tmp
(
	[TeamName]  VARCHAR(50)
	, [TeamFlag] VARCHAR(100)
)

 BEGIN TRAN

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
UNION ALL SELECT 'Finland','Finland.gif'
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
UNION ALL SELECT 'FYR Macedonia','NorthMacedonia.png'
UNION ALL SELECT 'Scotland','scotland.png'	
UNION ALL SELECT 'Slovakia','Slovakia.gif'
UNION ALL SELECT 'Hungary','Hungary.gif'

SELECT TMP.*, T.*
FROM dbo.Teams t
INNER JOIN #tmp AS TMP ON TMP.TeamName = t.TeamName

UPDATE T
SET T.TeamFlag = TMP.TeamFlag
	, T.ModifiedDateTime = GETDATE()
FROM dbo.Teams t
INNER JOIN #tmp AS TMP ON TMP.TeamName = t.TeamName

SELECT *
FROM dbo.Teams t
	

commit TRAN