Use Predictioncomp

IF (SELECT COUNT(1) FROM [dbo].[Leagues] WHERE LeagueName = 'Euro 2021 Group A') = 0
BEGIN

	INSERT INTO [dbo].[Leagues]
	(
		[LeagueName]
		,[ShortLeagueName]
		,[CreatedDateTime]
		,[ModifiedDateTime]
	)
	SELECT 'Euro 2021 Group A', 'A', GETDATE(), GETDATE()
	UNION ALL SELECT 'Euro 2021 Group B', 'B', GETDATE(), GETDATE()
	UNION ALL SELECT 'Euro 2021 Group C', 'C', GETDATE(), GETDATE()
	UNION ALL SELECT 'Euro 2021 Group D', 'D', GETDATE(), GETDATE()
	UNION ALL SELECT 'Euro 2021 Group E', 'E', GETDATE(), GETDATE()
	UNION ALL SELECT 'Euro 2021 Group F', 'F', GETDATE(), GETDATE()

END;