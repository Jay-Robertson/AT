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
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Mavo.Assets.Models.AssetContext context)
        {

            context.Templates.Add(new Template() { Id = 1, Name = "I'm a template!" });

            AssetCategory ladder = new AssetCategory { Id = 1, Name = "Ladders" };
            AssetCategory filters = new AssetCategory { Id = 2, Name = "Respirator Filters" };
            AssetCategory lumber = new AssetCategory { Id = 3, Name = "Lumber, MSC Materials" };
            AssetCategory poly = new AssetCategory { Id = 4, Name = "Poly/Disposal Materials" };
            AssetCategory hepa = new AssetCategory { Id = 5, Name = "HEPA's/Vacuums" };
            context.AssetCategories.AddOrUpdate(
                ladder,
                filters,
                lumber,
                poly,
                hepa
            );

            context.Assets.AddOrUpdate(
             new Asset { Id = 1, Inventory = 1, Kind = AssetKind.Consumable, Name = "Test Ladder Asset (Consumable)", Barcode = "1234", Category = ladder },
             new Asset { Id = 2, Inventory = 1, Kind = AssetKind.Serialized, Name = "Test Filter Asset (Serialized)", Barcode = "1234", Category = filters },
             new Asset { Id = 2, Inventory = 1, Kind = AssetKind.NotSerialized, Name = "Test Lumber Asset (NotSearilized)", Barcode = "1234", Category = lumber },
             new Asset { Id = 2, Inventory = 1, Kind = AssetKind.Consumable, Name = "Test poly Asset", Barcode = "1234", Category = poly },
             new Asset { Id = 2, Inventory = 1, Kind = AssetKind.Consumable, Name = "Test hepa Asset", Barcode = "1234", Category = hepa }
           );

            Customer target = new Customer
            {
                Id = 1,
                Name = "Target",
                CustomerNumber = "Cust1243",
                Address = new Address() { Address1 = "123 anywhere", City = "test", State = "ut", ZipCode = "55406" },
                ContactName = "Scott",
                PhoneNumber = "123-123-1234"
            };
            context.Customers.AddOrUpdate(
                target,
                new Customer
                {
                    Id = 2,
                    Name = "Walmart",
                    Address = new Address() { Address1 = "123 anywhere", City = "test", State = "ut", ZipCode = "55406" }
                }
            );

            context.Jobs.AddOrUpdate(
                new Job
                {
                    Id = 1,
                    Address = new Address()
                    {
                        Address1 = "1234 anywhere",
                        City = "SLC",
                        State = "UT",
                        ZipCode = "55406"
                    },
                    ContractAmount = 10m,
                    ContractNumber = "1234",
                    Customer = target,
                    Description = "This is a description",
                    ContractDate = DateTime.Now.AddDays(100),
                    EstimatedCompletionDate = DateTime.Now.AddYears(1),
                    ForemanNote = "This is a foreman note",
                    Foreman = new User() { FirstName = "Test", LastName = "Foreman", Email = "test@forman.com", EmployeeId = "12a", Role = UserRole.Foreman },
                    JobNumber = "4321",
                    JobSiteName = "Job Site 1",
                    PickupTime = DateTime.Now.AddDays(2),
                    ProjectManager = new User() { Id = 1, FirstName = "Scott", LastName = "ProjectManager", Email = "scott@redbranchsoftware.com", Role = UserRole.ProjectManager | UserRole.Foreman },
                    Name = "Ready to pickup",
                    Status = JobStatus.ReadyToPick
                }
            );
        }
    }
}
