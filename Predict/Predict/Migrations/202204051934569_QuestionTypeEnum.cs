namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionTypeEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuizQuestions", "AnswerTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuizQuestions", "AnswerTypeId", c => c.Short(nullable: false));
        }
    }
}
