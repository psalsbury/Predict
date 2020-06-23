USE Predictioncomp
GO

/* BEFORE THIS IS RUN, CREATE PETE@SALSBURY.CO.UK USER */

DECLARE @intEventId INT = 1
DECLARE @strAdminUserId NVARCHAR(128)
DECLARE @intDefaultPoolId INT

SELECT  @strAdminUserId = Id
FROM [dbo].[AspNetUsers]
WHERE Email = 'pete@salsbury.co.uk'

IF (SELECT COUNT(1) FROM dbo.Events WHERE DefaultPoolId = 0 AND Id = @intEventId AND @strAdminUserId IS NOT NULL) = 1
BEGIN

	INSERT INTO [dbo].[Pools]
	(
		PoolName
		, AdminPlayerId
		, EventId
		, JoinCode
		, InitialInfo
		, MemberInfo
		, FreezePredictions
		, EntryFee
		, FirstPercent
		, SecondPercent
		, ThirdPercent
		, NonPrizePercent
		, EmailNotifications
		, CorrectScorePoints
		, CorrectResultPoints
		, WinMarginPoints
		, KoLast16Points
		, KoLast8Points
		, KoLast4Points
		, KoLast2Points
		, KoLast1Points
		, CreatedDateTime
		, ModifiedDateTime
	)
	SELECT	'Global Pool'
		, @strAdminUserId
		, @intEventId
		, NULL	-- JoinCode
		, NULL	-- InitialInfo
		, NULL	-- MemberInfo
		, 0		-- FreezePredictions
		, NULL	-- EntryFee
		, NULL	-- FirstPercent
		, NULL	-- SecondPercent
		, NULL	-- ThirdPercent
		, NULL	-- NonPrizePercent
		, 0		-- EmailNotifications
		, 3		-- CorrectScorePoints
		, 1		-- CorrectResultPoints
		, 1		-- WinMarginPoints
		, 0		-- KoLast16Points
		, 0		-- KoLast8Points
		, 0		-- KoLast4Points
		, 0		-- KoLast2Points
		, 0		-- KoLast1Points
		, GETDATE()	-- CreatedDateTime
		, GETDATE()	--ModifiedDateTime

		SELECT @intDefaultPoolId = SCOPE_IDENTITY();

		UPDATE dbo.Events
		SET DefaultPoolId = @intDefaultPoolId
		WHERE Id = @intEventId;

		INSERT INTO [dbo].[PoolPlayers]
		(
			PoolId
			, PlayerId
			, AdminApprovedDateTime
			, PoolPosition
			, CorrectScore
			, CorrectResult
			, WinMargin
			, KoScore
			, BonusScore
			, TotalScore
			, CreatedDateTime
			, ModifiedDateTime
		)
		SELECT @intDefaultPoolId	-- PoolId
			, @strAdminUserId		-- PlayerId
			, GETDATE()				-- AdminApprovedDateTime
			, 0						-- PoolPosition
			, 0						-- CorrectScore
			, 0						-- CorrectResult
			, 0						-- WinMargin
			, 0						-- KoScore
			, 0						-- BonusScore
			, 0						-- TotalScore
			, GETDATE()				-- CreatedDateTime
			, GETDATE()				-- ModifiedDateTime
	
END;
