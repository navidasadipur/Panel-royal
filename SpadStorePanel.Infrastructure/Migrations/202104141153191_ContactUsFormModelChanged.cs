namespace SpadStorePanel.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactUsFormModelChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContactForms", "Phone", c => c.String(maxLength: 600));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContactForms", "Phone", c => c.String(nullable: false, maxLength: 600));
        }
    }
}
