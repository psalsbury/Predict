namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PoolChat : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PoolChats",
                c => new
                    {
                        PoolId = c.Int(nullable: false),
                        PlayerId = c.String(nullable: false, maxLength: 128),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Message = c.String(nullable: false, maxLength: 100),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.PoolId, t.PlayerId, t.CreatedDateTime })
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .ForeignKey("dbo.Pools", t => t.PoolId, cascadeDelete: true)
                .Index(t => t.PoolId)
                .Index(t => t.PlayerId);
            
            AddColumn("dbo.Events", "International", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PoolChats", "PoolId", "dbo.Pools");
            DropForeignKey("dbo.PoolChats", "PlayerId", "dbo.Players");
            DropIndex("dbo.PoolChats", new[] { "PlayerId" });
            DropIndex("dbo.PoolChats", new[] { "PoolId" });
            DropColumn("dbo.Events", "International");
            DropTable("dbo.PoolChats");
        }
    }
}
