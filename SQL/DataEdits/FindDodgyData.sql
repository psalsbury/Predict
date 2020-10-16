
/* Delete disabled entries */
delete 
from eventpools
where enabled =0

delete
from EventPlayers
where Enabled = 0

delete
from PoolPlayers
where enabled = 0

delete 
from eventpoolplayers
where Enabled = 0

/* Missing  eventpools rows */
insert into [dbo].[EventPools]
(
	[EventId]
	,[PoolId]
	,[Enabled]
	,[CreatedDateTime]
	,[ModifiedDateTime]
)
select distinct epp.eventid 
	, epp.poolid
	, 1
	, getdate()
	, getdate()
from eventpoolplayers epp
left outer join eventpools ep on ep.eventid = epp.eventid and ep.poolid = epp.poolid
where ep.CreatedDateTime is null
and epp.Enabled = 1

/* Missing  PoolPlayers rows */
select distinct epp.poolid, epp.PlayerId
from eventpoolplayers epp
left outer join PoolPlayers pp on pp.PoolId = epp.PoolId and pp.PlayerId = epp.PlayerId
where pp.CreatedDateTime is null
and epp.Enabled = 1

/* Missing  EventPlayers rows */
select distinct epp.eventid , epp.PlayerId
from eventpoolplayers epp
left outer join EventPlayers ep on ep.EventId = epp.EventId and ep.PlayerId = epp.PlayerId
where ep.CreatedDateTime is null
and epp.Enabled = 1

select *
from BonusQuestionPredictions

select *
from FixturePredictions
where EventId = 6
and  PlayerId = 'fc9ceaa0-67fc-4bba-9990-851f0283cc72'

select *
from eventpools
where eventid = 6

select *
from PoolPlayers
where PlayerId = 'fc9ceaa0-67fc-4bba-9990-851f0283cc72'

select *
from EventPlayers
where PlayerId = 'fc9ceaa0-67fc-4bba-9990-851f0283cc72'

select *
from EventPoolPlayers
where PlayerId = 'fc9ceaa0-67fc-4bba-9990-851f0283cc72'
and EventId = 6
