USE Predict
GO

IF (SELECT COUNT(1) FROM Teams WHERE TeamName = 'Arsenal') = 0
BEGIN

	INSERT INTO Predict.dbo.Teams
	(
		[TeamName]
		, [TeamFlag]
		, [CreatedDateTime]
		, [ModifiedDateTime]
	)
	SELECT 'Arsenal','Arsenal.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Aston Villa','AstonVilla.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Bournemouth','Bournemouth.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Brighton','Brighton.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Burnley','Burnley.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Chelsea','Chelsea.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Crystal Palace','CrystalPalace.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Everton','Everton.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Leicester City','LeicesterCity.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Liverpool','Liverpool.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Manchester City','ManchesterCity.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Manchester United','ManchesterUnited.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Newcastle United','NewcastleUnited.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Norwich City','NorwichCity.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Sheffield United','SheffieldUnited.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Southampton','Southampton.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Tottenham Hotspur','TottenhamHotspur.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Watford','Watford.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'West Ham','WestHam.png', GETDATE(), GETDATE()
	UNION ALL SELECT 'Wolves','Wolves.png', GETDATE(), GETDATE()
	
END;
