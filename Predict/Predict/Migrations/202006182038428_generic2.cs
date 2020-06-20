namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generic2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "DefaultPoolId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "DefaultPoolId");
        }
    }
}
