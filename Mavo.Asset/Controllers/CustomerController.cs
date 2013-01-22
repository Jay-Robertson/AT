using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Mavo.Assets.Models;

namespace Mavo.Assets.Controllers
{
    public class CustomerController : ApiController
    {
        private AssetContext db = new AssetContext();

        public IEnumerable<Customer> GetCustomers()
        {
            return db.Customers.AsEnumerable();
        }

        public Customer GetCustomer(string id)
        {
            Customer customer = db.Customers.FirstOrDefault(x => x.CustomerNumber == id);
            if (customer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return customer;
        }

        public Customer PostCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
            }
            else
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadGateway));

            return customer;
        }

        public HttpResponseMessage PutCustomer(string id, Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isNew = false;
                    var customerToSave = db.Customers.FirstOrDefault(x => x.CustomerNumber == id);
                    if (customerToSave == null)
                    {
                        customerToSave = new Customer();
                        customerToSave.CustomerNumber = id;
                        isNew = true;
                    }
                    customerToSave.Address = customer.Address;
                    customerToSave.ContactName = customer.ContactName;
                    customerToSave.Name = customer.Name;
                    customerToSave.PhoneNumber = customer.PhoneNumber;
                    if (isNew)
                    {
                        db.Customers.Add(customerToSave);
                    }
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}