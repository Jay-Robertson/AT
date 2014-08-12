using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TransactionsSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var from = DateTime.Now.Date;
            var to = from.AddDays(1);
            var url = ConfigurationManager.AppSettings["MavoUrl"];

            TransactionsSync.ProjectSync.Sync(url, from, to);
            TransactionsSync.TransactionSync.Sync(url, from, to);
        }

        
    }
}
