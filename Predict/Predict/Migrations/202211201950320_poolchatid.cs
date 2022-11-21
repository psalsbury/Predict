namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class poolchatid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PoolChats", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.PoolPlayers", "PlayerId", "dbo.Players");
            DropIndex("dbo.PoolChats", new[] { "PlayerId" });
            DropIndex("dbo.PoolPlayers", new[] { "PlayerId" });
            DropPrimaryKey("dbo.PoolChats");
            DropPrimaryKey("dbo.PoolPlayers");
            AddColumn("dbo.PoolChats", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PoolPlayers", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.PoolChats", "PlayerId", c => c.String(maxLength: 128));
            AlterColumn("dbo.PoolChats", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PoolPlayers", "PlayerId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.PoolChats", "Id");
            AddPrimaryKey("dbo.PoolPlayers", "Id");
            CreateIndex("dbo.PoolChats", "PlayerId");
            CreateIndex("dbo.PoolPlayers", "PlayerId");
            AddForeignKey("dbo.PoolChats", "PlayerId", "dbo.Players", "Id");
            AddForeignKey("dbo.PoolPlayers", "PlayerId", "dbo.Players", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PoolPlayers", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.PoolChats", "PlayerId", "dbo.Players");
            DropIndex("dbo.PoolPlayers", new[] { "PlayerId" });
            DropIndex("dbo.PoolChats", new[] { "PlayerId" });
            DropPrimaryKey("dbo.PoolPlayers");
            DropPrimaryKey("dbo.PoolChats");
            AlterColumn("dbo.PoolPlayers", "PlayerId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.PoolChats", "CreatedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PoolChats", "PlayerId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.PoolPlayers", "Id");
            DropColumn("dbo.PoolChats", "Id");
            AddPrimaryKey("dbo.PoolPlayers", new[] { "PoolId", "PlayerId" });
            AddPrimaryKey("dbo.PoolChats", new[] { "PoolId", "PlayerId", "CreatedDateTime" });
            CreateIndex("dbo.PoolPlayers", "PlayerId");
            CreateIndex("dbo.PoolChats", "PlayerId");
            AddForeignKey("dbo.PoolPlayers", "PlayerId", "dbo.Players", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PoolChats", "PlayerId", "dbo.Players", "Id", cascadeDelete: true);
        }
    }
}
