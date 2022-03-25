/* 
Migrate jokes from the wccomp database to predictioncomp
*/
INSERT INTO Predictioncomp.dbo.Jokes
(
	PlayerId
	,JokeText
	,JokePunchline
	,CreatedDateTime
	,ModifiedDateTime
)
SELECT 'e51699d7-7cf2-4565-905c-4a89c4f80063' -- PlayerID
	  ,[JokeText]
      ,[AnswerText]
	  ,GETDATE()
	  ,GETDATE()
  FROM [wccomp].[dbo].[tblJoke]


