namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bonus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BonusQuestionPredictions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BonusQuestionId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        PredictedAnswer = c.String(maxLength: 50),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BonusQuestions", t => t.BonusQuestionId, cascadeDelete: true)
                .Index(t => t.BonusQuestionId);
            
            CreateTable(
                "dbo.BonusQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Short(nullable: false),
                        Question = c.String(nullable: false, maxLength: 200),
                        Score = c.Int(nullable: false),
                        Answer = c.String(maxLength: 50),
                        ToBeAnsweredByDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BonusQuestionPredictions", "BonusQuestionId", "dbo.BonusQuestions");
            DropForeignKey("dbo.BonusQuestions", "EventId", "dbo.Events");
            DropIndex("dbo.BonusQuestions", new[] { "EventId" });
            DropIndex("dbo.BonusQuestionPredictions", new[] { "BonusQuestionId" });
            DropTable("dbo.BonusQuestions");
            DropTable("dbo.BonusQuestionPredictions");
        }
    }
}
