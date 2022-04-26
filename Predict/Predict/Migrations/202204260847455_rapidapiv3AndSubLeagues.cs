namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rapidapiv3AndSubLeagues : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeagueSubLeagues",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        LeagueId = c.Short(nullable: false),
                        SubLeagueName = c.String(nullable: false, maxLength: 25),
                        SubLeagueShortName = c.String(nullable: false, maxLength: 15),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leagues", t => t.LeagueId, cascadeDelete: true)
                .Index(t => t.LeagueId);
            
            CreateTable(
                "dbo.LeagueSubLeagueTeams",
                c => new
                    {
                        LeagueSubLeagueId = c.Short(nullable: false),
                        TeamId = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.LeagueSubLeagueId, t.TeamId })
                .ForeignKey("dbo.LeagueSubLeagues", t => t.LeagueSubLeagueId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.LeagueSubLeagueId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.RapidApiV3Country",
                c => new
                    {
                        CountryName = c.String(nullable: false, maxLength: 100),
                        CountryCode = c.String(maxLength: 10),
                        Flag = c.String(maxLength: 500),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.CountryName);
            
            CreateTable(
                "dbo.RapidApiV3League",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        Type = c.String(nullable: false, maxLength: 100),
                        Logo = c.String(nullable: false, maxLength: 500),
                        CountryName = c.String(nullable: false, maxLength: 100),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RapidApiV3Country", t => t.CountryName, cascadeDelete: true)
                .Index(t => t.CountryName);
            
            CreateTable(
                "dbo.RapidApiV3LeagueSeason",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RapidApiV3LeagueId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Current = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RapidApiV3League", t => t.RapidApiV3LeagueId, cascadeDelete: true)
                .Index(t => t.RapidApiV3LeagueId);
            
            AddColumn("dbo.Leagues", "RapidApiV3LeagueSeasonId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RapidApiV3LeagueSeason", "RapidApiV3LeagueId", "dbo.RapidApiV3League");
            DropForeignKey("dbo.RapidApiV3League", "CountryName", "dbo.RapidApiV3Country");
            DropForeignKey("dbo.LeagueSubLeagueTeams", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.LeagueSubLeagueTeams", "LeagueSubLeagueId", "dbo.LeagueSubLeagues");
            DropForeignKey("dbo.LeagueSubLeagues", "LeagueId", "dbo.Leagues");
            DropIndex("dbo.RapidApiV3LeagueSeason", new[] { "RapidApiV3LeagueId" });
            DropIndex("dbo.RapidApiV3League", new[] { "CountryName" });
            DropIndex("dbo.LeagueSubLeagueTeams", new[] { "TeamId" });
            DropIndex("dbo.LeagueSubLeagueTeams", new[] { "LeagueSubLeagueId" });
            DropIndex("dbo.LeagueSubLeagues", new[] { "LeagueId" });
            DropColumn("dbo.Leagues", "RapidApiV3LeagueSeasonId");
            DropTable("dbo.RapidApiV3LeagueSeason");
            DropTable("dbo.RapidApiV3League");
            DropTable("dbo.RapidApiV3Country");
            DropTable("dbo.LeagueSubLeagueTeams");
            DropTable("dbo.LeagueSubLeagues");
        }
    }
}
