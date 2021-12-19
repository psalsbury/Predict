namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Forum : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForumMessages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ForumTopicId = c.Long(nullable: false),
                        PlayerId = c.String(maxLength: 128),
                        ReplyToForumMessageId = c.Long(),
                        Message = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumTopics", t => t.ForumTopicId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId)
                .Index(t => t.ForumTopicId)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.ForumTopics",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PlayerId = c.String(maxLength: 128),
                        Topic = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.PlayerId)
                .Index(t => t.PlayerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumMessages", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.ForumMessages", "ForumTopicId", "dbo.ForumTopics");
            DropForeignKey("dbo.ForumTopics", "PlayerId", "dbo.Players");
            DropIndex("dbo.ForumTopics", new[] { "PlayerId" });
            DropIndex("dbo.ForumMessages", new[] { "PlayerId" });
            DropIndex("dbo.ForumMessages", new[] { "ForumTopicId" });
            DropTable("dbo.ForumTopics");
            DropTable("dbo.ForumMessages");
        }
    }
}
