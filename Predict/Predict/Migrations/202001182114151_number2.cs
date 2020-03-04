namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class number2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PoolPlayerPositionHistory", "ModifiedDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PoolPlayerPositionHistory", "ModifiedDateTime");
        }
    }
}
