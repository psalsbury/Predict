namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseDatesDateTime2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.EventGenerations");
            AlterColumn("dbo.EventGenerations", "BaseStartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EventGenerations", "BaseEndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddPrimaryKey("dbo.EventGenerations", new[] { "EventId", "LeagueEventGenerationId", "BaseStartDate" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.EventGenerations");
            AlterColumn("dbo.EventGenerations", "BaseEndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EventGenerations", "BaseStartDate", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.EventGenerations", new[] { "EventId", "LeagueEventGenerationId", "BaseStartDate" });
        }
    }
}
