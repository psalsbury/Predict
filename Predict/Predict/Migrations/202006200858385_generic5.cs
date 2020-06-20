namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generic5 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.EventPlayers", "EventId");
            AddForeignKey("dbo.EventPlayers", "EventId", "dbo.Events", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventPlayers", "EventId", "dbo.Events");
            DropIndex("dbo.EventPlayers", new[] { "EventId" });
        }
    }
}
