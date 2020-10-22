namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdddPoolPlayerEmailSent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PoolPlayers", "EmailSentToAdminDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PoolPlayers", "EmailSentToAdminDateTime");
        }
    }
}
