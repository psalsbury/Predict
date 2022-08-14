namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixtureStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fixtures", "RapidApiLongStatus", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fixtures", "RapidApiLongStatus");
        }
    }
}
