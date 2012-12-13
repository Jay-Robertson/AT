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

        public Customer GetCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return customer;
        }

        public HttpResponseMessage PutCustomer(string id, Customer customer)
        {
            if (ModelState.IsValid && id == customer.CustomerNumber)
            {
                var isNew = false;
                var customerToSave = db.Customers.FirstOrDefault(x => x.CustomerNumber == id);
                if (customerToSave == null)
                {
                    customerToSave = new Customer();
                    isNew = true;
                }
                customerToSave.Address = customer.Address;
                customer.ContactName = customer.ContactName;
                customer.CustomerNumber = id;
                customer.Name = customer.Name;
                customer.PhoneNumber = customer.PhoneNumber;
                if (isNew)
                    db.Customers.Add(customerToSave);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage PostCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, customer);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = customer.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage DeleteCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Customers.Remove(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, customer);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}