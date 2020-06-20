namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generic4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Pools", "EventId", "dbo.Events");
            DropIndex("dbo.Pools", new[] { "EventId" });
            DropPrimaryKey("dbo.EventPlayers");
            AlterColumn("dbo.EventPlayers", "EventId", c => c.Short(nullable: false));
            AlterColumn("dbo.Pools", "EventId", c => c.Short(nullable: false));
            AddPrimaryKey("dbo.EventPlayers", new[] { "EventId", "PlayerId" });
            CreateIndex("dbo.Pools", "EventId");
            AddForeignKey("dbo.Pools", "EventId", "dbo.Events", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pools", "EventId", "dbo.Events");
            DropIndex("dbo.Pools", new[] { "EventId" });
            DropPrimaryKey("dbo.EventPlayers");
            AlterColumn("dbo.Pools", "EventId", c => c.Short());
            AlterColumn("dbo.EventPlayers", "EventId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.EventPlayers", new[] { "EventId", "PlayerId" });
            CreateIndex("dbo.Pools", "EventId");
            AddForeignKey("dbo.Pools", "EventId", "dbo.Events", "Id");
        }
    }
}
