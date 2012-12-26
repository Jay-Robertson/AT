namespace Mavo.Assets.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Mavo.Assets.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Mavo.Assets.Models.AssetContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Mavo.Assets.Models.AssetContext context)
        {

            //context.Templates.Add(new Template() { Id = 1, Name = "I'm a template!" });


            //Customer target = new Customer
            //{
            //    Id = 1,
            //    Name = "Target",
            //    CustomerNumber = "Cust1243",
            //    Address = new Address() { Address1 = "123 anywhere", City = "test", State = "ut", ZipCode = "55406" },
            //    ContactName = "Scott",
            //    PhoneNumber = "123-123-1234"
            //};
            //context.Customers.AddOrUpdate(
            //    target,
            //    new Customer
            //    {
            //        Id = 2,
            //        Name = "Walmart",
            //        Address = new Address() { Address1 = "123 anywhere", City = "test", State = "ut", ZipCode = "55406" }
            //    },
            //     new Customer
            //    {
            //        Id = 3,
            //        Name = "Belay",
            //        Address = new Address() { Address1 = "658 Grand Avenue Suite 202", City = "Saint Paul", State = "MN", ZipCode = "55105" }
            //    }
            //);

            //context.Users.AddOrUpdate(
            //    new User() { Email = "projectmanager@mavo.com", EmployeeId = "pm1234", FirstName = "Project", LastName = "Manager", Role = UserRole.ProjectManager  },
            //    new User() { Email = "foreman@mavo.com", EmployeeId = "fm1234", FirstName = "Foreman", LastName = "Joe", Role = UserRole.Foreman  },
            //    new User() { Email = "warehousemanager@mavo.com", EmployeeId = "wm1234", FirstName = "Warehouse", LastName = "Manager", Role = UserRole.WarehouseManager  },
            //    new User() { Email = "warehousestaff@mavo.com", EmployeeId = "ws1234", FirstName = "Warehouse", LastName = "Staff", Role = UserRole.WarehouseStaff  },
            //    new User() { Email = "administrator@mavo.com", EmployeeId = "admin1234", FirstName = "Admin", LastName = "Joe", Role = UserRole.Administrator  }
            //    );

            //context.Jobs.AddOrUpdate(
            //    new Job
            //    {
            //        Id = 1,
            //        Address = new Address()
            //        {
            //            Address1 = "1234 anywhere",
            //            City = "SLC",
            //            State = "UT",
            //            ZipCode = "55406"
            //        },
            //        ContractAmount = 10m,
            //        ContractNumber = "1234",
            //        Customer = target,
            //        Description = "This is a description",
            //        ContractDate = DateTime.Now.AddDays(100),
            //        EstimatedCompletionDate = DateTime.Now.AddYears(1),
            //        ForemanNote = "This is a foreman note",
            //        Foreman = new User() { FirstName = "Test", LastName = "Foreman", Email = "test@forman.com", EmployeeId = "12a", Role = UserRole.Foreman },
            //        JobNumber = "4321",
            //        JobSiteName = "Job Site 1",
            //        PickupTime = DateTime.Now.AddDays(2),
            //        ProjectManager = new User() { Id = 1, FirstName = "Scott", LastName = "ProjectManager", Email = "scott@redbranchsoftware.com", Role = UserRole.ProjectManager | UserRole.Foreman },
            //        Name = "Ready to pickup",
            //        Status = JobStatus.ReadyToPick
            //    }
            //);
        }
    }
}
