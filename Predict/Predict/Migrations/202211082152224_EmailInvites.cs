namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailInvites : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailRequestToJoins",
                c => new
                    {
                        EventId = c.Short(nullable: false),
                        PoolId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        StatusId = c.Short(nullable: false),
                        SentDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.EventId, t.PoolId, t.PlayerId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .ForeignKey("dbo.Pools", t => t.PoolId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.PoolId)
                .Index(t => t.PlayerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailRequestToJoins", "PoolId", "dbo.Pools");
            DropForeignKey("dbo.EmailRequestToJoins", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.EmailRequestToJoins", "EventId", "dbo.Events");
            DropIndex("dbo.EmailRequestToJoins", new[] { "PlayerId" });
            DropIndex("dbo.EmailRequestToJoins", new[] { "PoolId" });
            DropIndex("dbo.EmailRequestToJoins", new[] { "EventId" });
            DropTable("dbo.EmailRequestToJoins");
        }
    }
}
