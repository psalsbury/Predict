namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastChatId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PoolPlayers", "LastViewedPoolChatId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PoolPlayers", "LastViewedPoolChatId");
        }
    }
}
