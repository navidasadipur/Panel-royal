namespace SpadStorePanel.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class articleModelChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "MainImage", c => c.String());
            AddColumn("dbo.Articles", "RightImage", c => c.String());
            AddColumn("dbo.Articles", "LeftImage", c => c.String());
            DropColumn("dbo.Articles", "Image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Image", c => c.String());
            DropColumn("dbo.Articles", "LeftImage");
            DropColumn("dbo.Articles", "RightImage");
            DropColumn("dbo.Articles", "MainImage");
        }
    }
}
