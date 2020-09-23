USE Predictioncomp
GO

/* BEFORE THIS IS RUN, CREATE PETE@SALSBURY.CO.UK USER */

DECLARE @strAdminUserId NVARCHAR(128)
DECLARE @intDefaultPoolId INT

SELECT  @strAdminUserId = Id
FROM [dbo].[AspNetUsers]
WHERE Email = 'pete@salsbury.co.uk'

IF (SELECT COUNT(1) FROM dbo.[Pools] WHERE PoolName = 'Global Pool') = 0
BEGIN

	INSERT INTO [dbo].[Pools]
	(
		PoolName
		, AdminPlayerId
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
		, DefaultPoolForEvent
		, CreatedDateTime
		, ModifiedDateTime
	)
	SELECT	'Global Pool'
		, @strAdminUserId
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
		, 5		-- CorrectScorePoints
		, 2		-- CorrectResultPoints
		, 1		-- WinMarginPoints
		, 0		-- KoLast16Points
		, 0		-- KoLast8Points
		, 0		-- KoLast4Points
		, 0		-- KoLast2Points
		, 0		-- KoLast1Points
		, 1		-- DefaultPoolForEvent
		, GETDATE()	-- CreatedDateTime
		, GETDATE()	--ModifiedDateTime

END;
