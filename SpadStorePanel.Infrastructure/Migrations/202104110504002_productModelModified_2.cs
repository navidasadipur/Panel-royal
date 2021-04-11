namespace SpadStorePanel.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productModelModified_2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "DescriptionOneTitle", c => c.String(maxLength: 600));
            AlterColumn("dbo.Products", "DescriptionTwoTitle", c => c.String(maxLength: 600));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "DescriptionTwoTitle", c => c.String(nullable: false, maxLength: 600));
            AlterColumn("dbo.Products", "DescriptionOneTitle", c => c.String(nullable: false, maxLength: 600));
        }
    }
}
