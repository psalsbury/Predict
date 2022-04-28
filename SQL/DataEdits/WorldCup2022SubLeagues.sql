/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[LeagueId]
      ,[SubLeagueName]
      ,[SubLeagueShortName]
      ,[CreatedDateTime]
      ,[ModifiedDateTime]
  FROM [Predictioncomp].[dbo].[LeagueSubLeagues]

select * from [dbo].[LeagueSubLeagueTeams]

  insert into [Predictioncomp].[dbo].[LeagueSubLeagues]
  select 19,'Group A', 'A', GETDATE(), GETDATE()

  insert into [Predictioncomp].[dbo].[LeagueSubLeagues]
  select 19,'Group C', 'C', GETDATE(), GETDATE()

  insert into [Predictioncomp].[dbo].[LeagueSubLeagues]
  select 19,'Group F', 'F', GETDATE(), GETDATE()

    insert into [Predictioncomp].[dbo].[LeagueSubLeagues]
  select 19,'Group G', 'G', GETDATE(), GETDATE()

  INSERT INTO [dbo].[LeagueSubLeagueTeams]
  SELECT 5, 874,  GETDATE(), GETDATE()
  UNION ALL   SELECT 5, 875,  GETDATE(), GETDATE()
  UNION ALL   SELECT 5, 872,  GETDATE(), GETDATE()
  UNION ALL   SELECT 5, 11,  GETDATE(), GETDATE()

    INSERT INTO [dbo].[LeagueSubLeagueTeams] --f
  SELECT 7, 2,  GETDATE(), GETDATE()
  UNION ALL   SELECT 7, 882,  GETDATE(), GETDATE()
  UNION ALL   SELECT 7, 880,  GETDATE(), GETDATE()
  UNION ALL   SELECT 7, 3,  GETDATE(), GETDATE()

      INSERT INTO [dbo].[LeagueSubLeagueTeams] --g
  SELECT 8, 887,  GETDATE(), GETDATE()
  UNION ALL   SELECT 8, 888,  GETDATE(), GETDATE()
  UNION ALL   SELECT 8, 17,  GETDATE(), GETDATE()
  UNION ALL   SELECT 8, 883,  GETDATE(), GETDATE()


  select *
  from teams
  where teamname = 'cameroon'
