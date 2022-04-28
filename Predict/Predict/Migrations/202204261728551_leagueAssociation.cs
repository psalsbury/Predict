namespace Predict.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class leagueAssociation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeagueEventGenerations", "TeamId", c => c.Int());
            AddColumn("dbo.EventsKo", "LinkedLeagueId", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventsKo", "LinkedLeagueId");
            DropColumn("dbo.LeagueEventGenerations", "TeamId");
        }
    }
}
