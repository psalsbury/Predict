
 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 1, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('qatar','ECUADOR','SENEGAL','NETHERLANDS')

 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 2, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('ENGLAND','IRAN','USA','WALES')

 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 3, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('aRGENTINA','SAUDI ARABIA','MEXICO','POLAND')

 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 4, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('FRANCE','AUSTRALIA','DENMARK','TUNISIA')

 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 5, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('SPAIN','COSTA RICA','GERMANY','JAPAN')


 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 6, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('BRAZIL','SERBIA','SWITZERLAND','CAMEROON')

 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 7, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('PORTUGAL','SWITZERLAND','SOUTH KOREA','GHANA')

 INSERT INTO [dbo].[LeagueSubLeagueTeams]
select 8, ID,  GETDATE(), GETDATE()
from teams
where TeamName IN ('BELGIUM','CANADA','MOROCCO','CROATIA')