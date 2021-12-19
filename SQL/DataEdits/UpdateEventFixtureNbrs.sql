with cte as
(
	SELECT EventId, COUNT(1) NbrFixtures
	FROM [dbo].[EventFixtures]
	GROUP BY EventId
)
update e
set e.Fixtures = cte.NbrFixtures
from Events e
inner join cte on cte.EventId = e.Id;

with cte as
(
SELECT EventId, COUNT(1) NbrFixtures
FROM [dbo].[BonusQuestions]
GROUP BY EventId
)
update e
set e.BonusQuestions = cte.NbrFixtures
from Events e
inner join cte on cte.EventId = e.Id;


with cte as
(
	SELECT EventId, COUNT(1) NbrFixtures
	FROM [dbo].[KoFixtures]
	GROUP BY EventId
)
update e
set e.KoFixtures = (cte.NbrFixtures*2)+1
from Events e
inner join cte on cte.EventId = e.Id;

WITH CTE AS
(
SELECT EventId, PlayerId, COUNT(1) Fixtures
FROM FixturePredictions FP
GROUP BY EventId, PlayerId
)
UPDATE EP
SET EP.[FixturePredictionsEntered] = cte.Fixtures
from EventPlayers EP
inner join cte on cte.EventId = ep.EventId and cte.PlayerId = ep.PlayerId


select *
from Events

