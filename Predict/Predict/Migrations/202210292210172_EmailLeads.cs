namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailLeads : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailLeads",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        PlayerName = c.String(nullable: false, maxLength: 100),
                        EmailAddress = c.String(nullable: false, maxLength: 100),
                        WasAdmin = c.String(nullable: false),
                        EmailDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailLeads");
        }
    }
}
