namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BonusQuestions", "BonusQuestionProcessed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Events", "Fixtures", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "KoFixtures", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "BonusQuestions", c => c.Int(nullable: false));
            AddColumn("dbo.EventPlayers", "FixturePredictionsEntered", c => c.Int(nullable: false));
            AddColumn("dbo.EventPlayers", "KoPredictionsEntered", c => c.Int(nullable: false));
            AddColumn("dbo.EventPlayers", "BonusPredictionsEntered", c => c.Int(nullable: false));
            AddColumn("dbo.KoFixtures", "Team1ResultProcessed", c => c.Boolean(nullable: false));
            AddColumn("dbo.KoFixtures", "Team2ResultProcessed", c => c.Boolean(nullable: false));
            DropColumn("dbo.EventPoolPlayers", "FixturePredictionsEntered");
            DropColumn("dbo.EventPoolPlayers", "KoPredictionsEntered");
            DropColumn("dbo.KoFixtures", "ResultProcessed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KoFixtures", "ResultProcessed", c => c.Boolean(nullable: false));
            AddColumn("dbo.EventPoolPlayers", "KoPredictionsEntered", c => c.Int());
            AddColumn("dbo.EventPoolPlayers", "FixturePredictionsEntered", c => c.Int());
            DropColumn("dbo.KoFixtures", "Team2ResultProcessed");
            DropColumn("dbo.KoFixtures", "Team1ResultProcessed");
            DropColumn("dbo.EventPlayers", "BonusPredictionsEntered");
            DropColumn("dbo.EventPlayers", "KoPredictionsEntered");
            DropColumn("dbo.EventPlayers", "FixturePredictionsEntered");
            DropColumn("dbo.Events", "BonusQuestions");
            DropColumn("dbo.Events", "KoFixtures");
            DropColumn("dbo.Events", "Fixtures");
            DropColumn("dbo.BonusQuestions", "BonusQuestionProcessed");
        }
    }
}
