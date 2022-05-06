/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[EventId]
      ,[Question]
      ,[Score]
      ,[Answer]
      ,[ToBeAnsweredByDateTime]
      ,[CreatedDateTime]
      ,[ModifiedDateTime]
      ,[BonusQuestionProcessed]
  FROM [Predictioncomp].[dbo].[BonusQuestions]

  insert into [Predictioncomp].[dbo].[BonusQuestions]
  select 218, 'Time of the first goal in the World Cup Final',6,null, '18 dec 2022 15:00', getdate(), getdate(), 0
  union all select 218, 'Total number of goals in both semi finals',6,null, '13 dec 2022 19:00', getdate(), getdate(), 0