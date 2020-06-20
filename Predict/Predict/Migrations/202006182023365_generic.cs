namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generic : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Players", "SupportTeamId", "dbo.Teams");
            DropIndex("dbo.Players", new[] { "SupportTeamId" });
            CreateTable(
                "dbo.EventPlayers",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.EventId, t.PlayerId });
            
            AddColumn("dbo.Events", "PlayerDeadlineDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Players", "SupportTeamId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "SupportTeamId", c => c.Int());
            DropColumn("dbo.Events", "PlayerDeadlineDateTime");
            DropTable("dbo.EventPlayers");
            CreateIndex("dbo.Players", "SupportTeamId");
            AddForeignKey("dbo.Players", "SupportTeamId", "dbo.Teams", "Id");
        }
    }
}
