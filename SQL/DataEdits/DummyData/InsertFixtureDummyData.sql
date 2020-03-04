use predict

select *
from KoFixtures

select *
from EventTeams

delete from  Fixtures
delete from EventTeams

insert into EventTeams
select top 24 
	1
	, Id
	, char((row_number() over(order by id)%6)+65)
	, getdate()
	, getdate()
from teams

;with cte as
(
select a.TeamId Team1, b.TeamId Team2, a.league, row_number() over(partition by a.league order by a.teamId*b.teamid) [row], rank() over(partition by a.league order by a.teamid*b.teamid) [rank], row_number() over(order by a.teamId*b.teamid) pos
from EventTeams a
inner join EventTeams b on a.League = b.League and a.TeamId <> b.TeamId
)
insert into fixtures
select 1
	, dateadd(day,pos,dateadd(year,1,getdate()))
	, team1
	, team2
	, null
	, null
	, getdate()
	, getdate()
from cte a
where [row] <> [rank]
order by league


