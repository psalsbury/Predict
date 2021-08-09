  
  BEGIN

	  UPDATE BQ
	  SET [Answer] = NULL
	  FROM [dbo].[BonusQuestions] BQ
	  WHERE [Id] = 1 /* Time of the first goal in the Euro 2021 Final */

  COMMIT TRAN




  BEGIN

	  UPDATE BQ
	  SET [Answer] = NULL
	  FROM [dbo].[BonusQuestions] BQ
	  WHERE [Id] = 2 /* Total number of goals in both semi finals */

  COMMIT TRAN
