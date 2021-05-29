namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupportTeamId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "SupportTeamId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "SupportTeamId");
        }
    }
}
