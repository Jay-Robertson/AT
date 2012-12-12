using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavo.Assets.Models;
using System.Data.Entity.Migrations;

namespace Mavo.Assets.Data
{
    //public interface IRepository
    //{
    //    AssetContext Context { get; }
    //    IEnumerable<Job> GetJobs();
    //     Job GetJobById(int id);
    //     List<Customer> GetCustomers();

    //     List<Job> GetReadyJobs();

    //     List<User> GetForemen();

    //     List<User> GetProjectManagers();

    //     Customer GetCustomer(int customerId);

    //     User GetUser(int userId);
    //}
    public class Repository 
    {
        private readonly AssetContext _Context;
        /// <summary>
        /// Initializes a new instance of the Repository class.
        /// </summary>
        public Repository(AssetContext context)
        {
            _Context = context;            
        }
        public IEnumerable<Job> GetJobs()
        {
            return Context.Jobs;
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
            return Context.Jobs.Where(x => x.Status == JobStatus.ReadyToPick).ToList();
        }


        public List<User> GetForemen()
        {
            return Context.Users.Where(x => (x.Role & UserRole.Foreman) == UserRole.Foreman).ToList();
        }

        public List<User> GetProjectManagers()
        {
            return Context.Users.Where(x => (x.Role & UserRole.ProjectManager) == UserRole.ProjectManager).ToList();
        }


        public Customer GetCustomer(int customerId)
        {
            return Context.Customers.FirstOrDefault(x => x.Id == customerId);
        }



        public User GetUser(int userId)
        {
            return Context.Users.FirstOrDefault(x => x.Id == userId);
        }

        public AssetContext Context
        {
            get { return this._Context; }
        }
    }
}
