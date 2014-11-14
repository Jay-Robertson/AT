using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Mavo.Assets.Models;

namespace Mavo.Assets.Controllers
{
    public class ProjectsController : ApiController
    {
        public class Project
        {
            public string CustomerNumber { get; set; }
            public string JobNumber { get; set; }
            public string Name { get; set; }
            public string Site { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public DateTime? EstimatedCompletionDate { get; set; }
            public decimal? EstimatedCost { get; set; }
            public DateTime? ContractStartDate { get; set; }
            public string JobContractNumber { get; set; }
            public Decimal? JobContractAmount { get; set; }
            public string ProjectManagerCode { get; set; }
        }

        private AssetContext db = new AssetContext();

        [HttpGet]
        public IEnumerable<Project> Get(DateTime from, DateTime to)
        {
            var a = db.Jobs.Where(x => x.CreatedDate >= from && x.CreatedDate < to).Select(x =>
                new Project
                {
                    CustomerNumber = x.Customer.CustomerNumber,
                    JobNumber = x.JobNumber,
                    Name = x.Name,
                    Address1 = x.Address.Address1,
                    Address2 = x.Address.Address2,
                    City = x.Address.City,
                    State = x.Address.State,
                    PostalCode = x.Address.ZipCode,
                    EstimatedCompletionDate = x.EstimatedCompletionDate,
                    ContractStartDate = x.ContractDate,
                    JobContractNumber = x.ContractNumber,
                    JobContractAmount = x.ContractAmount ?? x.EstimatedContractAmount,
                    ProjectManagerCode = x.ProjectManager.EmployeeId,
                    EstimatedCost = x.EstimatedCost,
                }
            );
            return a;
        }

    }

    public class AssetsController : ApiController
    {
        
        public class AssetTransaction
        {
            public DateTime Date { get; set; }
            public string JobNumber { get; set; }
            public string ItemNumber { get; set; }
            public int Quantity { get; set; }
        }

        private AssetContext db = new AssetContext();

        
        [HttpGet]
        public IEnumerable<AssetTransaction> Get(DateTime from, DateTime to)
        {
            var cx = db.Database.Connection;
            if (cx.State != ConnectionState.Open)
            {
                cx.Open();
            }
            var cmd = (SqlCommand)cx.CreateCommand();
            cmd.CommandText = @"
                --declare @from datetime = '1/1/2013'
                --declare @to datetime = '4/10/2013'

                select j.JobNumber, a.MavoItemNumber 'ItemNumber', aq.QuantityPicked 'Quantity', pa.Picked as 'Date'
                from PickedAsset pa
                join Jobs j on j.Id = pa.Job_Id
                join AssetsWithQuantity aq on aq.Id = pa.Id
                join Assets a on a.Id = aq.Asset_Id
                where a.Kind = 0 and aq.QuantityPicked > 0
                  and pa.Picked >= @from and pa.Picked < @to
                union
                select j.JobNumber, a.MavoItemNumber, aq.QuantityPicked * -1 as 'QuantityPicked', ra.Returned
                from ReturnedAsset ra
                join Jobs j on j.Id = ra.Job_Id
                join AssetsWithQuantity aq on aq.Id = ra.Id
                join Assets a on a.Id = aq.Asset_Id
                where a.Kind = 0 and aq.QuantityPicked > 0
                  and ra.Returned >= @from and ra.Returned < @to";
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            var rs = cmd.ExecuteReader();
            var a = new List<AssetTransaction>(200);
            while (rs.Read())
            {
                a.Add(new AssetTransaction
                {
                    Date = (DateTime)rs["Date"],
                    JobNumber = (String)rs["JobNumber"],
                    ItemNumber = (String)rs["ItemNumber"],
                    Quantity = (int)rs["Quantity"],
                });
            }
            return a;
        }
    }
}