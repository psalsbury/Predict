namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generic3 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.EventPlayers");
            AlterColumn("dbo.EventPlayers", "PlayerId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.EventPlayers", new[] { "EventId", "PlayerId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.EventPlayers");
            AlterColumn("dbo.EventPlayers", "PlayerId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.EventPlayers", new[] { "EventId", "PlayerId" });
        }
    }
}
