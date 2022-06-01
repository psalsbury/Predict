namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventNameTo26Chars : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "EventName", c => c.String(nullable: false, maxLength: 26));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "EventName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
