namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pete2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EventsKo", "FinalGoalTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EventsKo", "FinalGoalTime", c => c.Short());
        }
    }
}
