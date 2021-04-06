namespace SpadStorePanel.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductModelModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DescriptionOneTitle", c => c.String(nullable: false, maxLength: 600));
            AddColumn("dbo.Products", "DescriptionOneShortDescription", c => c.String(maxLength: 2000));
            AddColumn("dbo.Products", "DescriptionOneImage", c => c.String());
            AddColumn("dbo.Products", "DescriptionTwoTitle", c => c.String(nullable: false, maxLength: 600));
            AddColumn("dbo.Products", "DescriptionTwoShortDescription", c => c.String(maxLength: 2000));
            AddColumn("dbo.Products", "DescriptionTwoImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DescriptionTwoImage");
            DropColumn("dbo.Products", "DescriptionTwoShortDescription");
            DropColumn("dbo.Products", "DescriptionTwoTitle");
            DropColumn("dbo.Products", "DescriptionOneImage");
            DropColumn("dbo.Products", "DescriptionOneShortDescription");
            DropColumn("dbo.Products", "DescriptionOneTitle");
        }
    }
}
