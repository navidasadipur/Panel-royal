namespace SpadStorePanel.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OurTeamModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OurTeams", "FaceBookLink", c => c.String());
            AddColumn("dbo.OurTeams", "TwitterLink", c => c.String());
            AddColumn("dbo.OurTeams", "InstagramLink", c => c.String());
            DropColumn("dbo.OurTeams", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OurTeams", "Description", c => c.String(nullable: false));
            DropColumn("dbo.OurTeams", "InstagramLink");
            DropColumn("dbo.OurTeams", "TwitterLink");
            DropColumn("dbo.OurTeams", "FaceBookLink");
        }
    }
}
