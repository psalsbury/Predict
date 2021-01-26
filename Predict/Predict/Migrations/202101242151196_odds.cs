namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class odds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FixtureOddsByResults",
                c => new
                    {
                        RapidApiFixtureId = c.Int(nullable: false),
                        HomeOdds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DrawOdds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AwayOdds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.RapidApiFixtureId);
            
            CreateTable(
                "dbo.FixtureOddsByScores",
                c => new
                    {
                        RapidApiFixtureId = c.Int(nullable: false),
                        HomeScore = c.Short(nullable: false),
                        AwayScore = c.Short(nullable: false),
                        Odds = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.RapidApiFixtureId, t.HomeScore, t.AwayScore });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FixtureOddsByScores");
            DropTable("dbo.FixtureOddsByResults");
        }
    }
}
