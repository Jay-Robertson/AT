namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRequiredAttributeOnTemplateName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Template", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Template", "Name", c => c.String());
        }
    }
}
