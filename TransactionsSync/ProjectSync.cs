using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TransactionsSync
{
    public class Project
    {
        public string CustomerNumber { get; set; }
        public string JobNumber { get; set; }
        public string Name { get; set; }
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

    class ProjectSync
    {
        public static void Sync(String url, DateTime from, DateTime to)
        {
            Console.WriteLine("Querying for projects at '{0}'...", url);
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine("    .. selecting new projects from {0:d} to {1:d}", from, to);
            var response = client.GetAsync(String.Format("api/projects?from={0:d}&to={1:d}", from, to), HttpCompletionOption.ResponseContentRead);
            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("API call to Mavo site failed");
            }
            var json = response.Result.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<Project[]>(json.Result);
            Console.WriteLine("    .. retrieved {0} records to sync.", a.Length);

            var cs = ConfigurationManager.ConnectionStrings["Maxwell"].ConnectionString;
            Console.WriteLine("Connecting to ODBC source at '{0}'...", cs);
            var cx = new OdbcConnection(cs);

            var companyCode = ConfigurationManager.AppSettings["CO_CODE"];
            Console.WriteLine("    .. company code: {0}", companyCode);
            cx.Open();

            var cmd = cx.CreateCommand();
            cmd.CommandText = @"INSERT INTO CJCMX(CO_CODE, JOB_NO, JOB_NAME, JOB_ADDR_1, JOB_ADDR_2, JOB_ADDR_3) VALUES(?, ?, ?, ?, ?, ?)";
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters[0].Value = companyCode;

            var cmd2 = cx.CreateCommand();
            cmd2.CommandText = @"INSERT INTO CWIPX(
                    CO_CODE,
                    JOB_NO,
                    CUST_NO,
                    CERTIFY_PR_YN,
                    CONTRACT_DATE,
                    EST_COMPL_DATE,
                    JOB_CONTRACT_NO,
                    JOB_CNTRACT_AMT,
                    PROJ_MGR_CODE,
                    JOB_EST_COST_6
                )
                VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters.Add(cmd.CreateParameter());
            cmd2.Parameters[0].Value = companyCode;
            cmd2.Parameters[3].Value = "Y";
            try
            {
                foreach (var p in a)
                {
                    try
                    {
                        cmd.Parameters[1].Value = p.JobNumber ;
                        cmd.Parameters[2].Value = p.Name;
                        cmd.Parameters[3].Value = String.IsNullOrEmpty(p.Address1) ? "-" : p.Address1;
                        cmd.Parameters[4].Value = String.IsNullOrEmpty(p.Address2) ? "-" : p.Address2;
                        cmd.Parameters[5].Value = String.Format("{0}, {1} {2}", p.City, p.State, p.PostalCode);
                        cmd.ExecuteNonQuery();

                        cmd2.Parameters[1].Value = p.JobNumber;
                        cmd2.Parameters[2].Value = String.IsNullOrEmpty(p.CustomerNumber) ? "-" : p.CustomerNumber;
                        cmd2.Parameters[4].Value = p.ContractStartDate.HasValue ? p.ContractStartDate.Value.ToString("yyMMdd") : "-";
                        cmd2.Parameters[5].Value = p.EstimatedCompletionDate.HasValue ? p.EstimatedCompletionDate.Value.ToString("yyMMdd") : "-";
                        cmd2.Parameters[6].Value = String.IsNullOrEmpty(p.JobContractNumber) ? "-" : p.JobContractNumber;
                        cmd2.Parameters[7].Value = p.JobContractAmount.GetValueOrDefault(0).ToString();
                        cmd2.Parameters[8].Value = p.ProjectManagerCode ?? "";
                        cmd2.Parameters[9].Value = p.EstimatedCost.GetValueOrDefault(0).ToString();
                        cmd2.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to write record for Job Number {0}: {1}", p.JobNumber, ex.Message);
                    }
                }
                cmd.Dispose();
                //cmd2.Dispose();
                cx.Close();
                Console.WriteLine("    .. job done.");
            }
            catch (OdbcException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
