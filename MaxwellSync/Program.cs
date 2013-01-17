using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Mavo.Assets.Models;

namespace MaxwellSync
{
    class Program
    {
        static Customer ConvertToMavoCustomer(MaxwellCustomer c)
        {
            var x = new Customer();
            x.CustomerNumber = c.CustomerNumber;
            x.Name = c.CustomerName;
            if (String.IsNullOrWhiteSpace(x.Name))
            {
                x.Name = "(unknown customer)";
            }
            x.Address = new Address();
            x.Address.Address1 = c.Address1;
            if (!String.IsNullOrWhiteSpace(c.Address2))
            {
                x.Address.Address2 = c.Address2;
            }
            if (!String.IsNullOrWhiteSpace(c.Address3))
            {
                var cityStateZipParts = c.Address3.Split(' ', '-');
                var zipParts = new List<string>();
                int zipPart;
                var i = cityStateZipParts.Length;
                while (i > 0 && int.TryParse(cityStateZipParts[--i], out zipPart))
                {
                    zipParts.Insert(0, cityStateZipParts[i]);
                }
                x.Address.ZipCode = String.Join("-", zipParts);
                if (i > 0)
                {
                    x.Address.State = cityStateZipParts[i];
                    x.Address.City = cityStateZipParts[i - 1];
                }
                else if (i == 0)
                {
                    x.Address.City = cityStateZipParts[i];
                }

            }
            if (null == x.Address.Address1 &&
                null != x.Address.Address2)
            {
                x.Address.Address1 = x.Address.Address2;
                x.Address.Address2 = null;
            }            
            x.ContactName = c.ContactName;
            x.PhoneNumber = c.PhoneNumber;
            return x;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to Maxwell...");
            var cx = new MaxwellConnection();
            Console.Write("Loading customers...");
            var customers = cx.FindCustomers();
            Console.WriteLine(" found {0} records.", customers.Count);

            var client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["MavoUrl"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var i = 0;
            var failed = 0;
            foreach (var c in customers)
            {
                if (i++ % 100 == 0)
                {
                    Console.WriteLine(" ... uploading record {0} of {1}.", i, customers.Count);
                }
                var x = ConvertToMavoCustomer(c);
                var result = client.PutAsJsonAsync("/api/customer/" + c.CustomerNumber, x).Result;
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    failed++;
                    Console.WriteLine(" ... failed to save record for customer number [{0}] with result code {1}.", x.CustomerNumber, result.StatusCode);
                    if (failed > 10)
                    {
                        Console.WriteLine("Too many failed records, giving up.");
                        return;
                    }
                }
            }
            Console.WriteLine("Complete!");
        }
    }
}
