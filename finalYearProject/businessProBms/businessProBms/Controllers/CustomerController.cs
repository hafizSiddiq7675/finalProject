using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using businessProBms.Models;
using System.Net;
namespace businessProBms.Controllers
{
    public class CustomerController : Controller
    {
        private businessProBmsEntities db = new businessProBmsEntities();
        //
        // GET: /Customer/Customers
        public ActionResult Customers(int?page)
        {
            ViewBag.customers = (db.Customers.ToList().ToPagedList(page ?? 1, 8));
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Customers([Bind(Include="code,name,contact,address,creditLimit")] Customer customer)
        {
            if(ModelState.IsValid)
            {
                Customer c = db.Customers.Find(customer.code);
                if(c==null)
                {
                    db.Customers.Add(customer);
                }
                else
                {
                    var cDB = db.Customers.SingleOrDefault(m => m.code == customer.code);
                    cDB.code = customer.code;
                    cDB.name = customer.name;
                    cDB.contact = customer.contact;
                    cDB.address = customer.address;
                    cDB.creditLimit = customer.creditLimit;
                }
                db.SaveChanges();
                return RedirectToAction("Customers");
            }
            return View(customer);
        }
        public ActionResult Edit(int? id, int? page)
        {
            ViewBag.customers = (db.Customers.ToList().ToPagedList(page ?? 1, 8));
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer c = db.Customers.Find(id);
            if(id==null)
            {
                return HttpNotFound();
            }
            return View("Customers", c);
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Vendor v = db.Vendors.SingleOrDefault(m => m.code == id);
            if (v != null)
            {
                db.Vendors.Remove(v);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}