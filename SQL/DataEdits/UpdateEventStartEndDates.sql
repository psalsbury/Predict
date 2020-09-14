USE predictioncomp


CREATE TABLE #tmp
(
	EventId INT
	,StartDateTime DATETIME2
	,EndDateTime DATETIME2
)

INSERT INTO #tmp
(EventId,StartDateTime,EndDateTime)
SELECT E.Id
	, MIN(F.FixtureDateTime) AS MinDate
	, MAX(F.FixtureDateTime) AS MaxDate
FROM Events E
INNER JOIN EventFixtures AS EF ON EF.EventId = E.Id
INNER JOIN [dbo].[Fixtures] AS F ON F.Id = EF.FixtureId
WHERE E.Id = @intEventId
GROUP BY E.Id;

WITH CTE AS
(
	SELECT E.Id
		, MIN(F.FixtureDateTime) AS MinDate
		, MAX(F.FixtureDateTime) AS MaxDate
	FROM Events E
	INNER JOIN [dbo].[KoFixtures] AS F ON F.EventId =E.Id
	GROUP BY E.Id
)
UPDATE T
SET StartDateTime = CASE WHEN ISNULL(CTE.MinDate,T.StartDateTime) < T.StartDateTime THEN CTE.MinDate ELSE T.StartDateTime END
	, EndDateTime = CASE WHEN ISNULL(CTE.MinDate,T.EndDateTime) > T.EndDateTime THEN CTE.MaxDate ELSE T.EndDateTime END
FROM #TMP T
INNER JOIN CTE ON CTE.Id = T.EventId

UPDATE E
SET E.StartDateTime = T.StartDateTime
	, E.EndDateTime = T.EndDateTime
FROM Events E
INNER JOIN #tmp T ON T.EventId = E.Id


