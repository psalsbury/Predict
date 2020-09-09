namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BonusQuestionPredictions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BonusQuestionId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        PredictedAnswer = c.String(maxLength: 50),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BonusQuestions", t => t.BonusQuestionId, cascadeDelete: true)
                .Index(t => t.BonusQuestionId);
            
            CreateTable(
                "dbo.BonusQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Short(nullable: false),
                        Question = c.String(nullable: false, maxLength: 200),
                        Score = c.Int(nullable: false),
                        Answer = c.String(maxLength: 50),
                        ToBeAnsweredByDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        EventName = c.String(nullable: false, maxLength: 50),
                        EventDescription = c.String(maxLength: 100),
                        DefaultPoolId = c.Int(nullable: false),
                        StartDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventFixtures",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        FixtureId = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.EventId, t.FixtureId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Fixtures", t => t.FixtureId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.FixtureId);
            
            CreateTable(
                "dbo.Fixtures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FixtureDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        HomeTeamId = c.Int(),
                        AwayTeamId = c.Int(),
                        HomeResult = c.Short(),
                        AwayResult = c.Short(),
                        LeagueId = c.Short(nullable: false),
                        ResultProcessed = c.Boolean(nullable: false),
                        RapidApiFixtureId = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.AwayTeamId)
                .ForeignKey("dbo.Teams", t => t.HomeTeamId)
                .ForeignKey("dbo.Leagues", t => t.LeagueId, cascadeDelete: true)
                .Index(t => t.HomeTeamId)
                .Index(t => t.AwayTeamId)
                .Index(t => t.LeagueId);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamName = c.String(nullable: false, maxLength: 50),
                        TeamFlag = c.String(nullable: false, maxLength: 100),
                        RapidApiTeamId = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Leagues",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        LeagueName = c.String(nullable: false, maxLength: 25),
                        ShortLeagueName = c.String(nullable: false, maxLength: 10),
                        RapidApiLeagueId = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventsKo",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        KoStageFirstRoundQty = c.Short(),
                        WinningTeamId = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Teams", t => t.WinningTeamId)
                .Index(t => t.EventId)
                .Index(t => t.WinningTeamId);
            
            CreateTable(
                "dbo.EventPlayers",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        Enabled = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.EventId, t.PlayerId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.EventPools",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        PoolId = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.EventId, t.PoolId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Pools", t => t.PoolId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.PoolId);
            
            CreateTable(
                "dbo.Pools",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PoolName = c.String(nullable: false, maxLength: 50),
                        AdminPlayerId = c.String(maxLength: 128),
                        JoinCode = c.String(maxLength: 50),
                        InitialInfo = c.String(maxLength: 1000),
                        MemberInfo = c.String(maxLength: 1000),
                        FreezePredictions = c.Boolean(nullable: false),
                        EntryFee = c.Single(),
                        FirstPercent = c.Single(),
                        SecondPercent = c.Single(),
                        ThirdPercent = c.Single(),
                        NonPrizePercent = c.Single(),
                        EmailNotifications = c.Boolean(nullable: false),
                        CorrectScorePoints = c.Int(nullable: false),
                        CorrectResultPoints = c.Int(nullable: false),
                        WinMarginPoints = c.Int(nullable: false),
                        KoLast16Points = c.Int(),
                        KoLast8Points = c.Int(),
                        KoLast4Points = c.Int(),
                        KoLast2Points = c.Int(),
                        KoLast1Points = c.Int(),
                        DefaultPoolForEvent = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.AdminPlayerId)
                .Index(t => t.AdminPlayerId);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PlayerName = c.String(nullable: false, maxLength: 100),
                        DisplayName = c.String(nullable: false, maxLength: 30),
                        PremiumPlayer = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EmailConfirmedDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.FixturePredictions",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        FixtureId = c.Int(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        HomePrediction = c.Short(nullable: false),
                        AwayPrediction = c.Short(nullable: false),
                        CorrectScore = c.Boolean(),
                        CorrectResult = c.Boolean(),
                        CorrectWinMargin = c.Boolean(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventFixtures", t => new { t.EventId, t.FixtureId }, cascadeDelete: true)
                .Index(t => new { t.EventId, t.FixtureId });
            
            CreateTable(
                "dbo.KoFixturePredictions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        KoFixtureId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        Team1Id = c.Int(),
                        Team2Id = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KoFixtures", t => t.KoFixtureId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team1Id)
                .ForeignKey("dbo.Teams", t => t.Team2Id)
                .Index(t => t.KoFixtureId)
                .Index(t => t.Team1Id)
                .Index(t => t.Team2Id);
            
            CreateTable(
                "dbo.KoFixtures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Short(nullable: false),
                        FixtureDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RoundOf = c.Short(nullable: false),
                        Position = c.Short(nullable: false),
                        Team1FromLeagueId = c.Short(),
                        Team1FromLeaguePosition = c.Int(),
                        Team1FromKoFixtureId = c.Int(),
                        Team2FromLeagueId = c.Short(),
                        Team2FromLeaguePosition = c.Int(),
                        Team2FromKoFixtureId = c.Int(),
                        Team1Id = c.Int(),
                        Team2Id = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
                        Id = c.Long(nullable: false, identity: true),
                        EventId = c.Short(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        TeamId = c.Int(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.TeamId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.PoolPlayerPositionHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PoolId = c.Int(nullable: false),
                        EventId = c.Short(nullable: false),
                        PlayerId = c.String(maxLength: 128),
                        PositionDate = c.DateTime(nullable: false, storeType: "date"),
                        PoolPosition = c.Short(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        ModifiedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PoolPlayers", t => new { t.PoolId, t.PlayerId, t.EventId })
                .Index(t => new { t.PoolId, t.PlayerId, t.EventId });
            
            CreateTable(
                "dbo.PoolPlayers",
                c => new
                    {
                        PoolId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        EventId = c.Short(nullable: false),
                        AdminApprovedDateTime = c.DateTime(),
                        PoolPosition = c.Short(nullable: false),
                        CorrectScore = c.Short(nullable: false),
                        CorrectResult = c.Short(nullable: false),
                        WinMargin = c.Short(nullable: false),
                        KoScore = c.Short(nullable: false),
                        BonusScore = c.Short(nullable: false),
                        TotalScore = c.Short(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        ModifiedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.PoolId, t.PlayerId, t.EventId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .ForeignKey("dbo.Pools", t => t.PoolId, cascadeDelete: true)
                .Index(t => t.PoolId)
                .Index(t => t.PlayerId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.SiteSettings",
                c => new
                    {
                        SittingName = c.String(nullable: false, maxLength: 25),
                        SettingValue = c.String(nullable: false, maxLength: 200),
                        CreatedDateTime = c.DateTime(nullable: false),
                        ModifiedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SittingName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PoolPlayerPositionHistory", new[] { "PoolId", "PlayerId", "EventId" }, "dbo.PoolPlayers");
            DropForeignKey("dbo.PoolPlayers", "PoolId", "dbo.Pools");
            DropForeignKey("dbo.PoolPlayers", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.PoolPlayers", "EventId", "dbo.Events");
            DropForeignKey("dbo.KoWinningTeamPredictions", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.KoFixturePredictions", "Team2Id", "dbo.Teams");
            DropForeignKey("dbo.KoFixturePredictions", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.KoFixturePredictions", "KoFixtureId", "dbo.KoFixtures");
            DropForeignKey("dbo.KoFixtures", "Team2Id", "dbo.Teams");
            DropForeignKey("dbo.KoFixtures", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.FixturePredictions", new[] { "EventId", "FixtureId" }, "dbo.EventFixtures");
            DropForeignKey("dbo.EventPools", "PoolId", "dbo.Pools");
            DropForeignKey("dbo.Pools", "AdminPlayerId", "dbo.Players");
            DropForeignKey("dbo.Players", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventPools", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventPlayers", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventsKo", "WinningTeamId", "dbo.Teams");
            DropForeignKey("dbo.EventsKo", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventFixtures", "FixtureId", "dbo.Fixtures");
            DropForeignKey("dbo.Fixtures", "LeagueId", "dbo.Leagues");
            DropForeignKey("dbo.Fixtures", "HomeTeamId", "dbo.Teams");
            DropForeignKey("dbo.Fixtures", "AwayTeamId", "dbo.Teams");
            DropForeignKey("dbo.EventFixtures", "EventId", "dbo.Events");
            DropForeignKey("dbo.BonusQuestionPredictions", "BonusQuestionId", "dbo.BonusQuestions");
            DropForeignKey("dbo.BonusQuestions", "EventId", "dbo.Events");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PoolPlayers", new[] { "EventId" });
            DropIndex("dbo.PoolPlayers", new[] { "PlayerId" });
            DropIndex("dbo.PoolPlayers", new[] { "PoolId" });
            DropIndex("dbo.PoolPlayerPositionHistory", new[] { "PoolId", "PlayerId", "EventId" });
            DropIndex("dbo.KoWinningTeamPredictions", new[] { "TeamId" });
            DropIndex("dbo.KoFixtures", new[] { "Team2Id" });
            DropIndex("dbo.KoFixtures", new[] { "Team1Id" });
            DropIndex("dbo.KoFixturePredictions", new[] { "Team2Id" });
            DropIndex("dbo.KoFixturePredictions", new[] { "Team1Id" });
            DropIndex("dbo.KoFixturePredictions", new[] { "KoFixtureId" });
            DropIndex("dbo.FixturePredictions", new[] { "EventId", "FixtureId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Players", new[] { "Id" });
            DropIndex("dbo.Pools", new[] { "AdminPlayerId" });
            DropIndex("dbo.EventPools", new[] { "PoolId" });
            DropIndex("dbo.EventPools", new[] { "EventId" });
            DropIndex("dbo.EventPlayers", new[] { "EventId" });
            DropIndex("dbo.EventsKo", new[] { "WinningTeamId" });
            DropIndex("dbo.EventsKo", new[] { "EventId" });
            DropIndex("dbo.Fixtures", new[] { "LeagueId" });
            DropIndex("dbo.Fixtures", new[] { "AwayTeamId" });
            DropIndex("dbo.Fixtures", new[] { "HomeTeamId" });
            DropIndex("dbo.EventFixtures", new[] { "FixtureId" });
            DropIndex("dbo.EventFixtures", new[] { "EventId" });
            DropIndex("dbo.BonusQuestions", new[] { "EventId" });
            DropIndex("dbo.BonusQuestionPredictions", new[] { "BonusQuestionId" });
            DropTable("dbo.SiteSettings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PoolPlayers");
            DropTable("dbo.PoolPlayerPositionHistory");
            DropTable("dbo.KoWinningTeamPredictions");
            DropTable("dbo.KoFixtures");
            DropTable("dbo.KoFixturePredictions");
            DropTable("dbo.FixturePredictions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Players");
            DropTable("dbo.Pools");
            DropTable("dbo.EventPools");
            DropTable("dbo.EventPlayers");
            DropTable("dbo.EventsKo");
            DropTable("dbo.Leagues");
            DropTable("dbo.Teams");
            DropTable("dbo.Fixtures");
            DropTable("dbo.EventFixtures");
            DropTable("dbo.Events");
            DropTable("dbo.BonusQuestions");
            DropTable("dbo.BonusQuestionPredictions");
        }
    }
}
