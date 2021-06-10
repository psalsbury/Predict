namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PredsEntered : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventPoolPlayers", "FixturePredictionsEntered", c => c.Int());
            AddColumn("dbo.EventPoolPlayers", "KoPredictionsEntered", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventPoolPlayers", "KoPredictionsEntered");
            DropColumn("dbo.EventPoolPlayers", "FixturePredictionsEntered");
        }
    }
}
