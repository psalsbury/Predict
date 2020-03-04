select * 
from KoFixtures

DELETE FROM KoFixtures

declare @int INT = 1
declare @posn INT
DECLARE @Team1FromLeague VARCHAR(30)
DECLARE @Team1FromLeaguePosition INT
DECLARE @Team2FromLeague VARCHAR(30)
DECLARE @Team2FromLeaguePosition INT

WHILE @int <= 8
BEGIN

if @int=1
begin /* game  4*/
	SELECT @posn = 4
	SELECT @Team1FromLeague = 'A', @Team1FromLeaguePosition = 1	
	SELECT @Team2FromLeague = 'C', @Team2FromLeaguePosition = 2
end else if @int=2
begin /* game 8 */
	SELECT @posn = 8
	SELECT @Team1FromLeague = 'A', @Team1FromLeaguePosition = 2
	SELECT @Team2FromLeague = 'B', @Team2FromLeaguePosition = 2
end else if @int=3
begin /* game 3 */
	SELECT @posn = 3
	SELECT @Team1FromLeague = 'B', @Team1FromLeaguePosition = 1	
	SELECT @Team2FromLeague = 'ADEF', @Team2FromLeaguePosition = 3
end else if @int=4
begin /* game 7 */
	SELECT @posn = 7
	SELECT @Team1FromLeague = 'C', @Team1FromLeaguePosition = 1	
	SELECT @Team2FromLeague = 'DEF', @Team2FromLeaguePosition = 3
end else if @int=5
begin /* game 1 */
	SELECT @posn = 1
	SELECT @Team1FromLeague = 'F', @Team1FromLeaguePosition = 1	
	SELECT @Team2FromLeague = 'ABC', @Team2FromLeaguePosition = 3
end else if @int=6
begin /* game 2*/
	SELECT @posn = 2
	SELECT @Team1FromLeague = 'D', @Team1FromLeaguePosition = 2
	SELECT @Team2FromLeague = 'E', @Team2FromLeaguePosition = 2
end else if @int=7
begin /* game 5 */
	SELECT @posn = 5
	SELECT @Team1FromLeague = 'E', @Team1FromLeaguePosition = 1	
	SELECT @Team2FromLeague = 'ABCD', @Team2FromLeaguePosition = 3
end else if @int=8
begin /* game 6 */
	SELECT @posn = 6
	SELECT @Team1FromLeague = 'D', @Team1FromLeaguePosition = 1	
	SELECT @Team2FromLeague = 'F', @Team2FromLeaguePosition = 2
end


	INSERT INTO KoFixtures
	(EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, Team1FromLeague
	, Team1FromLeaguePosition
	, Team2FromLeague
	, Team2FromLeaguePosition
	, CreatedDateTime
	, ModifiedDateTime
	)
	SELECT 1
	, DATEADD(DAY,@int,DATEADD(YEAR,1,GETDATE()))
	, 16
	, @posn
	, @Team1FromLeague
	, @Team1FromLeaguePosition
	, @Team2FromLeague
	, @Team2FromLeaguePosition
	, GETDATE()
	, GETDATE()
	

	SET @int = @int + 1
END

SET @int = 1
WHILE @int <= 4
BEGIN

	INSERT INTO KoFixtures
	(EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, Team1FromKoFixtureId
	, Team2FromKoFixtureId
	, CreatedDateTime
	, ModifiedDateTime
	)
	SELECT 1
	, DATEADD(DAY,@int+20,DATEADD(YEAR,1,GETDATE()))
	, 8
	, @int
	, (@int*2)-1
	, (@int*2)
	, GETDATE()
	, GETDATE()

	SET @int = @int + 1
END


SET @int = 1
WHILE @int <= 2
BEGIN

	INSERT INTO KoFixtures
	(EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, Team1FromKoFixtureId
	, Team2FromKoFixtureId
	, CreatedDateTime
	, ModifiedDateTime
	)
	SELECT 1
	, DATEADD(DAY,@int+30,DATEADD(YEAR,1,GETDATE()))
	, 4
	, @int
	, 16+(@int*2)-1
	, 16+(@int*2)
	, GETDATE()
	, GETDATE()

	SET @int = @int + 1
END


SET @int = 1
	INSERT INTO KoFixtures
	(EventId
	, FixtureDateTime
	, RoundOf
	, Position
	, Team1FromKoFixtureId
	, Team2FromKoFixtureId
	, CreatedDateTime
	, ModifiedDateTime
	)
	SELECT 1
	, DATEADD(DAY,@int+50,DATEADD(YEAR,1,GETDATE()))
	, 2
	, @int
	, 24
	, 25
	, GETDATE()
	, GETDATE()


select * 
from KoFixtures
