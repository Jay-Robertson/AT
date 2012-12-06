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
            context.Assets.AddOrUpdate(
              new Asset { Id = 1, Inventory = 1, Kind = AssetKind.Consumable, Name = "Test Asset", Serial = "1234" }
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
                    Customer = new Customer() { Id = 1, Name = "Test Customer" },
                    Description = "This is a description",
                    ContractDate = DateTime.Now.AddDays(100),
                    EstimatedCompletionDate = DateTime.Now.AddYears(1),
                    ForemanNote = "This is a foreman note",
                    Foreman = new User() { FirstName = "Test", LastName = "Foreman", Email = "test@forman.com", EmployeeId = "12a", Role = UserRole.Foreman },
                    JobNumber = "4321",
                    JobSiteName = "Job Site 1",
                    PickupTime = DateTime.Now.AddDays(2),
                    ProjectManager = new User() { Id = 1, FirstName = "Scott", LastName = "ProjectManager", Email = "scott@redbranchsoftware.com", Role = UserRole.ProjectManager | UserRole.Foreman },
                    Name = "Test Job",
                    Status = JobStatus.ReadyToPick
                }
            );
        }
    }
}
