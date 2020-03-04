USE Predict
GO

IF (SELECT COUNT(1) FROM Teams) = 0
BEGIN

	INSERT INTO Predict.dbo.Teams
	(
		[TeamName]
		, [TeamFlag]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT 'Austria','Austria.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Belgium','Belgium.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Croatia','Croatia.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Czech Republic','CzechRepublic.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Denmark','Denmark.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'England','England.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Finland','Finland.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'France','France.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Germany','Germany.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Italy','Italy.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Netherlands','Netherlands.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Poland','Poland.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Portugal','Portugal.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Russia','Russia.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Spain','Spain.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Sweden','Sweden.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Switzerland','Switzerland.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Turkey','Turkey.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Ukraine','Ukraine.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'Wales','Wales.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'PlayOffWinnerA','PlayOffWinnerA.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'PlayOffWinnerB','PlayOffWinnerB.gif', GETDATE(), GETDATE()	
	UNION ALL SELECT 'PlayOffWinnerC','PlayOffWinnerC.gif', GETDATE(), GETDATE()
	UNION ALL SELECT 'PlayOffWinnerD','PlayOffWinnerD.gif', GETDATE(), GETDATE()
	
END;