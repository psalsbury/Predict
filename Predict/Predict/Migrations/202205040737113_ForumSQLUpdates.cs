namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForumSQLUpdates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ForumMessages", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.ForumTopics", "PlayerId", "dbo.Players");
            DropIndex("dbo.ForumMessages", new[] { "PlayerId" });
            DropIndex("dbo.ForumTopics", new[] { "PlayerId" });
            AlterColumn("dbo.ForumMessages", "PlayerId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ForumMessages", "Message", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.ForumTopics", "PlayerId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ForumMessages", "PlayerId");
            CreateIndex("dbo.ForumTopics", "PlayerId");
            AddForeignKey("dbo.ForumMessages", "PlayerId", "dbo.Players", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ForumTopics", "PlayerId", "dbo.Players", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumTopics", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.ForumMessages", "PlayerId", "dbo.Players");
            DropIndex("dbo.ForumTopics", new[] { "PlayerId" });
            DropIndex("dbo.ForumMessages", new[] { "PlayerId" });
            AlterColumn("dbo.ForumTopics", "PlayerId", c => c.String(maxLength: 128));
            AlterColumn("dbo.ForumMessages", "Message", c => c.String());
            AlterColumn("dbo.ForumMessages", "PlayerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ForumTopics", "PlayerId");
            CreateIndex("dbo.ForumMessages", "PlayerId");
            AddForeignKey("dbo.ForumTopics", "PlayerId", "dbo.Players", "Id");
            AddForeignKey("dbo.ForumMessages", "PlayerId", "dbo.Players", "Id");
        }
    }
}
