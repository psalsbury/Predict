namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JokesAndQuiz : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JokeRatings",
                c => new
                    {
                        JokeId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        PlayerJokeRating = c.Short(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.JokeId, t.PlayerId })
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.Jokes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        JokeText = c.String(nullable: false, maxLength: 500),
                        JokePunchline = c.String(nullable: false, maxLength: 500),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.QuizQuestionAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuizQuestionId = c.Int(nullable: false),
                        AnswerText = c.String(maxLength: 100),
                        IsCorrectAnswer = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizQuestions", t => t.QuizQuestionId, cascadeDelete: true)
                .Index(t => t.QuizQuestionId);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerId = c.String(maxLength: 128),
                        QuestionText = c.String(nullable: false, maxLength: 250),
                        AnswerTypeId = c.Short(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.PlayerId)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.QuizQuestionPlayerAnswers",
                c => new
                    {
                        QuizQuestionId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        AnswerGiven = c.String(nullable: false, maxLength: 100),
                        IsCorrect = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.QuizQuestionId, t.PlayerId })
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuizQuestionPlayerAnswers", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.QuizQuestionAnswers", "QuizQuestionId", "dbo.QuizQuestions");
            DropForeignKey("dbo.QuizQuestions", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.Jokes", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.JokeRatings", "PlayerId", "dbo.Players");
            DropIndex("dbo.QuizQuestionPlayerAnswers", new[] { "PlayerId" });
            DropIndex("dbo.QuizQuestions", new[] { "PlayerId" });
            DropIndex("dbo.QuizQuestionAnswers", new[] { "QuizQuestionId" });
            DropIndex("dbo.Jokes", new[] { "PlayerId" });
            DropIndex("dbo.JokeRatings", new[] { "PlayerId" });
            DropTable("dbo.QuizQuestionPlayerAnswers");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.QuizQuestionAnswers");
            DropTable("dbo.Jokes");
            DropTable("dbo.JokeRatings");
        }
    }
}
