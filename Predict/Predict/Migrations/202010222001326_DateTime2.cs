namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class DateTime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EventPoolPlayerPositionHistory", "CreatedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EventPoolPlayerPositionHistory", "ModifiedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EventPoolPlayers", "CreatedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EventPoolPlayers", "ModifiedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PoolPlayers", "CreatedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PoolPlayers", "ModifiedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SiteSettings", "CreatedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SiteSettings", "ModifiedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PoolPlayers", "EmailSentToAdminDateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));

        }

        public override void Down()
        {
            AlterColumn("dbo.SiteSettings", "ModifiedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SiteSettings", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PoolPlayers", "ModifiedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PoolPlayers", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EventPoolPlayers", "ModifiedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EventPoolPlayers", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EventPoolPlayerPositionHistory", "ModifiedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EventPoolPlayerPositionHistory", "CreatedDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PoolPlayers", "EmailSentToAdminDateTime", c => c.DateTime(nullable: false));

        }
    }
}
