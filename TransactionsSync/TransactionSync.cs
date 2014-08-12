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
    public class TransactionSync
    {
        public class AssetTransaction
        {
            public DateTime Date { get; set; }
            public string JobNumber { get; set; }
            public string ItemNumber { get; set; }
            public int Quantity { get; set; }
        }

        public static void Sync(String url, DateTime from, DateTime to)
        {
            Console.WriteLine("Querying services at '{0}'...", url);
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine("    .. selecting transactions from {0:d} to {1:d}", from, to);
            var response = client.GetAsync(String.Format("api/assets?from={0:d}&to={1:d}", from, to), HttpCompletionOption.ResponseContentRead);
            if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("API call to Mavo site failed");
            }
            var json = response.Result.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<AssetTransaction[]>(json.Result);
            Console.WriteLine("    .. retrieved {0} records to sync.", a.Length);

            var cs = ConfigurationManager.ConnectionStrings["Maxwell"].ConnectionString;
            Console.WriteLine("Connecting to ODBC source at '{0}'...", cs);
            var cx = new OdbcConnection(cs);

            var companyCode = ConfigurationManager.AppSettings["CO_CODE"];
            Console.WriteLine("    .. company code: {0}", companyCode);

            var requisitionNumber = from.ToString("yyMMdd");
            Console.WriteLine("    .. requisition number: {0}", requisitionNumber);

            var requisitionDate = from.ToString("yyyyMMdd");
            Console.WriteLine("    .. requisition date: {0}", requisitionDate);


            cx.Open();
            var cmd = cx.CreateCommand();
            cmd.CommandText = @"INSERT INTO CINRX(CO_CODE, REQUISITION_NO, LINE_NO, INV_ITEM_NO, MTL_REQ_QTY, JOB_NO, REQUISITION_DTE) VALUES(?, ?, ?, ?, ?, ?, ?)";
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters.Add(cmd.CreateParameter());
            cmd.Parameters[0].Value = companyCode;
            cmd.Parameters[1].Value = requisitionNumber;
            cmd.Parameters[6].Value = requisitionDate;
            try
            {
                var line = 0;
                foreach (var assetTransaction in a)
                {
                    line++;
                    cmd.Parameters[2].Value = line;
                    cmd.Parameters[3].Value = assetTransaction.ItemNumber;
                    cmd.Parameters[4].Value = assetTransaction.Quantity;
                    cmd.Parameters[5].Value = assetTransaction.JobNumber;
                    cmd.ExecuteNonQuery();
                }
                cmd.Dispose();
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
