using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavo.Assets.Models;

namespace Mavo.Assets.Data
{
    public interface IRepository
    {
         List<Job> GetJobs();
         Job GetJobById(int id);
         List<Customer> GetCustomers();

         List<Job> GetReadyJobs();
    }
    public class Repository : IRepository
    {
        private readonly AssetContext Context;
        /// <summary>
        /// Initializes a new instance of the Repository class.
        /// </summary>
        public Repository(AssetContext context)
        {
            Context = context;            
        }
        public List<Job> GetJobs()
        {
            throw new NotImplementedException();
        }

        public Job GetJobById(int id)
        {
            return Context.Jobs.FirstOrDefault(x => x.Id == id);
        }

        public List<Customer> GetCustomers()
        {
            return Context.Customers.ToList();
        }



        public List<Job> GetReadyJobs()
        {
            return Context.Jobs.Where(x => x.PickupTime >= DateTime.Now).ToList();
        }
    }
}
