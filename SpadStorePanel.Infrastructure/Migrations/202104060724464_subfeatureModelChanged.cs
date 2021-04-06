namespace SpadStorePanel.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subfeatureModelChanged : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubFeatures", "FeatureId", "dbo.Features");
            DropIndex("dbo.SubFeatures", new[] { "FeatureId" });
            AlterColumn("dbo.SubFeatures", "FeatureId", c => c.Int(nullable: false));
            CreateIndex("dbo.SubFeatures", "FeatureId");
            AddForeignKey("dbo.SubFeatures", "FeatureId", "dbo.Features", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubFeatures", "FeatureId", "dbo.Features");
            DropIndex("dbo.SubFeatures", new[] { "FeatureId" });
            AlterColumn("dbo.SubFeatures", "FeatureId", c => c.Int());
            CreateIndex("dbo.SubFeatures", "FeatureId");
            AddForeignKey("dbo.SubFeatures", "FeatureId", "dbo.Features", "Id");
        }
    }
}
