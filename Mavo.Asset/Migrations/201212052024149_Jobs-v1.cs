namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jobsv1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JobNumber = c.String(nullable: false),
                        JobSiteName = c.String(nullable: false),
                        Address_Address1 = c.String(nullable: false),
                        Address_Address2 = c.String(),
                        Address_City = c.String(nullable: false),
                        Address_State = c.String(nullable: false),
                        Address_ZipCode = c.String(nullable: false),
                        ContractNumber = c.String(),
                        ContractAmount = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                        ForemanNote = c.String(),
                        PickupTime = c.DateTime(nullable: false),
                        ContractDate = c.DateTime(nullable: false),
                        EstimatedCompletionDate = c.DateTime(nullable: false),
                        Customer_Id = c.Int(),
                        ProjectManager_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.Users", t => t.ProjectManager_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.ProjectManager_Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Jobs", new[] { "ProjectManager_Id" });
            DropIndex("dbo.Jobs", new[] { "Customer_Id" });
            DropForeignKey("dbo.Jobs", "ProjectManager_Id", "dbo.Users");
            DropForeignKey("dbo.Jobs", "Customer_Id", "dbo.Customers");
            DropTable("dbo.Customers");
            DropTable("dbo.Jobs");
        }
    }
}
