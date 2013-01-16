using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxwellSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var cx = new MaxwellConnection();
            foreach (var c in cx.ImportCustomers())
            {
                Console.WriteLine("Customer: {0} (#{1})", c.Name, c.Number);
            }
        }
    }
}
