using System.Data.Entity.Migrations;

namespace Predict.Migrations
{
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.BonusQuestionPredictions",
                    c => new
                    {
                        Id = c.Int(false, true),
                        BonusQuestionId = c.Int(false),
                        PlayerId = c.String(false, 128),
                        PredictedAnswer = c.String(maxLength: 50),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BonusQuestions", t => t.BonusQuestionId, true)
                .Index(t => t.BonusQuestionId);

            CreateTable(
                    "dbo.BonusQuestions",
                    c => new
                    {
                        Id = c.Int(false, true),
                        EventId = c.Short(false),
                        Question = c.String(false, 200),
                        Score = c.Int(false),
                        Answer = c.String(maxLength: 50),
                        ToBeAnsweredByDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, true)
                .Index(t => t.EventId);

            CreateTable(
                    "dbo.Events",
                    c => new
                    {
                        Id = c.Short(false, true),
                        EventName = c.String(false, 100),
                        EventStartDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        DefaultPoolId = c.Int(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.EventsKo",
                    c => new
                    {
                        EventId = c.Short(false),
                        KoStageFirstRoundQty = c.Short(),
                        WinningTeamId = c.Int(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Teams", t => t.WinningTeamId)
                .Index(t => t.EventId)
                .Index(t => t.WinningTeamId);

            CreateTable(
                    "dbo.Teams",
                    c => new
                    {
                        Id = c.Int(false, true),
                        TeamName = c.String(false, 50),
                        TeamFlag = c.String(false, 100),
                        AnimatedTeamFlag = c.String(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.EventPlayers",
                    c => new
                    {
                        EventId = c.Short(false),
                        PlayerId = c.String(false, 128),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => new {t.EventId, t.PlayerId})
                .ForeignKey("dbo.Events", t => t.EventId, true)
                .Index(t => t.EventId);

            CreateTable(
                    "dbo.EventTeams",
                    c => new
                    {
                        EventId = c.Int(false),
                        TeamId = c.Int(false),
                        League = c.String(maxLength: 50),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => new {t.EventId, t.TeamId})
                .ForeignKey("dbo.Teams", t => t.TeamId, true)
                .Index(t => t.TeamId);

            CreateTable(
                    "dbo.FixturePredictions",
                    c => new
                    {
                        Id = c.Long(false, true),
                        FixtureId = c.Int(false),
                        PlayerId = c.String(false, 128),
                        HomePrediction = c.Short(false),
                        AwayPrediction = c.Short(false),
                        CorrectScore = c.Boolean(),
                        CorrectResult = c.Boolean(),
                        CorrectWinMargin = c.Boolean(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fixtures", t => t.FixtureId, true)
                .Index(t => t.FixtureId);

            CreateTable(
                    "dbo.Fixtures",
                    c => new
                    {
                        Id = c.Int(false, true),
                        EventId = c.Short(false),
                        FixtureDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        HomeTeamId = c.Int(),
                        AwayTeamId = c.Int(),
                        HomeResult = c.Short(),
                        AwayResult = c.Short(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.AwayTeamId)
                .ForeignKey("dbo.Events", t => t.EventId, true)
                .ForeignKey("dbo.Teams", t => t.HomeTeamId)
                .Index(t => t.EventId)
                .Index(t => t.HomeTeamId)
                .Index(t => t.AwayTeamId);

            CreateTable(
                    "dbo.KoFixturePredictions",
                    c => new
                    {
                        Id = c.Long(false, true),
                        KoFixtureId = c.Int(false),
                        PlayerId = c.String(false, 128),
                        Team1Id = c.Int(),
                        Team2Id = c.Int(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KoFixtures", t => t.KoFixtureId, true)
                .ForeignKey("dbo.Teams", t => t.Team1Id)
                .ForeignKey("dbo.Teams", t => t.Team2Id)
                .Index(t => t.KoFixtureId)
                .Index(t => t.Team1Id)
                .Index(t => t.Team2Id);

            CreateTable(
                    "dbo.KoFixtures",
                    c => new
                    {
                        Id = c.Int(false, true),
                        EventId = c.Short(false),
                        FixtureDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        RoundOf = c.Short(false),
                        Position = c.Short(false),
                        Team1FromLeague = c.String(maxLength: 50),
                        Team1FromLeaguePosition = c.Int(),
                        Team1FromKoFixtureId = c.Int(),
                        Team2FromLeague = c.String(maxLength: 50),
                        Team2FromLeaguePosition = c.Int(),
                        Team2FromKoFixtureId = c.Int(),
                        Team1Id = c.Int(),
                        Team2Id = c.Int(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.Team1Id)
                .ForeignKey("dbo.Teams", t => t.Team2Id)
                .Index(t => t.Team1Id)
                .Index(t => t.Team2Id);

            CreateTable(
                    "dbo.KoWinningTeamPredictions",
                    c => new
                    {
                        Id = c.Long(false, true),
                        EventId = c.Short(false),
                        PlayerId = c.String(false, 128),
                        TeamId = c.Int(),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .Index(t => t.TeamId);

            CreateTable(
                    "dbo.Messages",
                    c => new
                    {
                        Id = c.Int(false, true),
                        Subject = c.String(false),
                        MessageToPost = c.String(false),
                        From = c.String(),
                        DatePosted = c.DateTime(false)
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.Players",
                    c => new
                    {
                        Id = c.String(false, 128),
                        PlayerName = c.String(false, 100),
                        DisplayName = c.String(false, 30),
                        PremiumPlayer = c.Boolean(false),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        EmailConfirmedDateTime = c.DateTime(precision: 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);

            CreateTable(
                    "dbo.AspNetUsers",
                    c => new
                    {
                        Id = c.String(false, 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(false),
                        TwoFactorEnabled = c.Boolean(false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(false),
                        AccessFailedCount = c.Int(false),
                        UserName = c.String(false, 256)
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                    "dbo.AspNetUserClaims",
                    c => new
                    {
                        Id = c.Int(false, true),
                        UserId = c.String(false, 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String()
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, true)
                .Index(t => t.UserId);

            CreateTable(
                    "dbo.AspNetUserLogins",
                    c => new
                    {
                        LoginProvider = c.String(false, 128),
                        ProviderKey = c.String(false, 128),
                        UserId = c.String(false, 128)
                    })
                .PrimaryKey(t => new {t.LoginProvider, t.ProviderKey, t.UserId})
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, true)
                .Index(t => t.UserId);

            CreateTable(
                    "dbo.AspNetUserRoles",
                    c => new
                    {
                        UserId = c.String(false, 128),
                        RoleId = c.String(false, 128)
                    })
                .PrimaryKey(t => new {t.UserId, t.RoleId})
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                    "dbo.PoolPlayerPositionHistory",
                    c => new
                    {
                        Id = c.Long(false, true),
                        PoolId = c.Int(false),
                        PlayerId = c.String(maxLength: 128),
                        PositionDate = c.DateTime(false, storeType: "date"),
                        PoolPosition = c.Short(false),
                        CreatedDateTime = c.DateTime(false),
                        ModifiedDateTime = c.DateTime(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PoolPlayers", t => new {t.PoolId, t.PlayerId})
                .Index(t => new {t.PoolId, t.PlayerId});

            CreateTable(
                    "dbo.PoolPlayers",
                    c => new
                    {
                        PoolId = c.Int(false),
                        PlayerId = c.String(false, 128),
                        AdminApprovedDateTime = c.DateTime(),
                        PoolPosition = c.Short(false),
                        CorrectScore = c.Short(false),
                        CorrectResult = c.Short(false),
                        WinMargin = c.Short(false),
                        KoScore = c.Short(false),
                        BonusScore = c.Short(false),
                        TotalScore = c.Short(false),
                        CreatedDateTime = c.DateTime(false),
                        ModifiedDateTime = c.DateTime(false)
                    })
                .PrimaryKey(t => new {t.PoolId, t.PlayerId})
                .ForeignKey("dbo.Players", t => t.PlayerId, true)
                .ForeignKey("dbo.Pools", t => t.PoolId, true)
                .Index(t => t.PoolId)
                .Index(t => t.PlayerId);

            CreateTable(
                    "dbo.Pools",
                    c => new
                    {
                        Id = c.Int(false, true),
                        PoolName = c.String(false, 50),
                        AdminPlayerId = c.String(maxLength: 128),
                        EventId = c.Short(false),
                        JoinCode = c.String(maxLength: 50),
                        InitialInfo = c.String(maxLength: 1000),
                        MemberInfo = c.String(maxLength: 1000),
                        FreezePredictions = c.Boolean(false),
                        EntryFee = c.Single(),
                        FirstPercent = c.Single(),
                        SecondPercent = c.Single(),
                        ThirdPercent = c.Single(),
                        NonPrizePercent = c.Single(),
                        EmailNotifications = c.Boolean(false),
                        CorrectScorePoints = c.Int(false),
                        CorrectResultPoints = c.Int(false),
                        WinMarginPoints = c.Int(false),
                        KoLast16Points = c.Int(false),
                        KoLast8Points = c.Int(false),
                        KoLast4Points = c.Int(false),
                        KoLast2Points = c.Int(false),
                        KoLast1Points = c.Int(false),
                        CreatedDateTime = c.DateTime(false, 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(false, 7, storeType: "datetime2")
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.AdminPlayerId)
                .ForeignKey("dbo.Events", t => t.EventId, true)
                .Index(t => t.AdminPlayerId)
                .Index(t => t.EventId);

            CreateTable(
                    "dbo.Replies",
                    c => new
                    {
                        Id = c.Int(false, true),
                        MessageId = c.Int(false),
                        ReplyFrom = c.String(),
                        ReplyMessage = c.String(false),
                        ReplyDateTime = c.DateTime(false)
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.AspNetRoles",
                    c => new
                    {
                        Id = c.String(false, 128),
                        Name = c.String(false, 256)
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PoolPlayerPositionHistory", new[] {"PoolId", "PlayerId"}, "dbo.PoolPlayers");
            DropForeignKey("dbo.PoolPlayers", "PoolId", "dbo.Pools");
            DropForeignKey("dbo.Pools", "EventId", "dbo.Events");
            DropForeignKey("dbo.Pools", "AdminPlayerId", "dbo.Players");
            DropForeignKey("dbo.PoolPlayers", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.Players", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.KoWinningTeamPredictions", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.KoFixturePredictions", "Team2Id", "dbo.Teams");
            DropForeignKey("dbo.KoFixturePredictions", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.KoFixturePredictions", "KoFixtureId", "dbo.KoFixtures");
            DropForeignKey("dbo.KoFixtures", "Team2Id", "dbo.Teams");
            DropForeignKey("dbo.KoFixtures", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.FixturePredictions", "FixtureId", "dbo.Fixtures");
            DropForeignKey("dbo.Fixtures", "HomeTeamId", "dbo.Teams");
            DropForeignKey("dbo.Fixtures", "EventId", "dbo.Events");
            DropForeignKey("dbo.Fixtures", "AwayTeamId", "dbo.Teams");
            DropForeignKey("dbo.EventTeams", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.EventPlayers", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventsKo", "WinningTeamId", "dbo.Teams");
            DropForeignKey("dbo.EventsKo", "EventId", "dbo.Events");
            DropForeignKey("dbo.BonusQuestionPredictions", "BonusQuestionId", "dbo.BonusQuestions");
            DropForeignKey("dbo.BonusQuestions", "EventId", "dbo.Events");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Pools", new[] {"EventId"});
            DropIndex("dbo.Pools", new[] {"AdminPlayerId"});
            DropIndex("dbo.PoolPlayers", new[] {"PlayerId"});
            DropIndex("dbo.PoolPlayers", new[] {"PoolId"});
            DropIndex("dbo.PoolPlayerPositionHistory", new[] {"PoolId", "PlayerId"});
            DropIndex("dbo.AspNetUserRoles", new[] {"RoleId"});
            DropIndex("dbo.AspNetUserRoles", new[] {"UserId"});
            DropIndex("dbo.AspNetUserLogins", new[] {"UserId"});
            DropIndex("dbo.AspNetUserClaims", new[] {"UserId"});
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Players", new[] {"Id"});
            DropIndex("dbo.KoWinningTeamPredictions", new[] {"TeamId"});
            DropIndex("dbo.KoFixtures", new[] {"Team2Id"});
            DropIndex("dbo.KoFixtures", new[] {"Team1Id"});
            DropIndex("dbo.KoFixturePredictions", new[] {"Team2Id"});
            DropIndex("dbo.KoFixturePredictions", new[] {"Team1Id"});
            DropIndex("dbo.KoFixturePredictions", new[] {"KoFixtureId"});
            DropIndex("dbo.Fixtures", new[] {"AwayTeamId"});
            DropIndex("dbo.Fixtures", new[] {"HomeTeamId"});
            DropIndex("dbo.Fixtures", new[] {"EventId"});
            DropIndex("dbo.FixturePredictions", new[] {"FixtureId"});
            DropIndex("dbo.EventTeams", new[] {"TeamId"});
            DropIndex("dbo.EventPlayers", new[] {"EventId"});
            DropIndex("dbo.EventsKo", new[] {"WinningTeamId"});
            DropIndex("dbo.EventsKo", new[] {"EventId"});
            DropIndex("dbo.BonusQuestions", new[] {"EventId"});
            DropIndex("dbo.BonusQuestionPredictions", new[] {"BonusQuestionId"});
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Replies");
            DropTable("dbo.Pools");
            DropTable("dbo.PoolPlayers");
            DropTable("dbo.PoolPlayerPositionHistory");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Players");
            DropTable("dbo.Messages");
            DropTable("dbo.KoWinningTeamPredictions");
            DropTable("dbo.KoFixtures");
            DropTable("dbo.KoFixturePredictions");
            DropTable("dbo.Fixtures");
            DropTable("dbo.FixturePredictions");
            DropTable("dbo.EventTeams");
            DropTable("dbo.EventPlayers");
            DropTable("dbo.Teams");
            DropTable("dbo.EventsKo");
            DropTable("dbo.Events");
            DropTable("dbo.BonusQuestions");
            DropTable("dbo.BonusQuestionPredictions");
        }
    }
}