namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoGen : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventGenerations",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        LeagueEventGenerationId = c.Short(nullable: false),
                        BaseStartDate = c.DateTime(nullable: false),
                        BaseEndDate = c.DateTime(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.EventId, t.LeagueEventGenerationId, t.BaseStartDate })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.LeagueEventGenerations", t => t.LeagueEventGenerationId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.LeagueEventGenerationId);
            
            CreateTable(
                "dbo.LeagueEventGenerations",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        LeagueId = c.Short(nullable: false),
                        GenerationFrequencyId = c.Short(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leagues", t => t.LeagueId, cascadeDelete: true)
                .Index(t => t.LeagueId);
            
            AddColumn("dbo.Leagues", "DailyRapidApiCheck", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventGenerations", "LeagueEventGenerationId", "dbo.LeagueEventGenerations");
            DropForeignKey("dbo.LeagueEventGenerations", "LeagueId", "dbo.Leagues");
            DropForeignKey("dbo.EventGenerations", "EventId", "dbo.Events");
            DropIndex("dbo.LeagueEventGenerations", new[] { "LeagueId" });
            DropIndex("dbo.EventGenerations", new[] { "LeagueEventGenerationId" });
            DropIndex("dbo.EventGenerations", new[] { "EventId" });
            DropColumn("dbo.Leagues", "DailyRapidApiCheck");
            DropTable("dbo.LeagueEventGenerations");
            DropTable("dbo.EventGenerations");
        }
    }
}
