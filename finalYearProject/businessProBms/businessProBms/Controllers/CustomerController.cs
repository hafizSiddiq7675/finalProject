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
            Customer c = new Customer();
            var maxId = db.Customers.ToList().OrderByDescending(r => r.customerCode).FirstOrDefault();
            c.customerCode = maxId == null ? 1 : (maxId.customerCode) + 1;
            ViewBag.customers = (db.Customers.ToList().ToPagedList(page ?? 1, 8));
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Customers([Bind(Include="customerCode,name,contact,address,creditLimit")] Customer customer)
        {
            if(ModelState.IsValid)
            {
                Customer c = db.Customers.Find(customer.customerCode);
                ExpenseAccount ex = new ExpenseAccount();
                if(c==null)
                {
                    var maxCode = db.ExpenseAccounts.ToList().OrderByDescending(r => r.code).FirstOrDefault();
                    ex.code = maxCode == null ? 1 : (maxCode.code) + 1;
                    ex.accountType = "Assets";
                    ex.description = "Customers Accounts";
                    ex.isGroup = false;
                    ex.name = customer.name;
                    ex.parentCode = 4; // This is Manual entry 
                    ex.openingCredit = 0;
                    ex.openingCredit = 0;
                    db.ExpenseAccounts.Add(ex);
                    db.SaveChanges();
                    customer.chartOfAccCode = ex.code;
                    db.Customers.Add(customer);
                }
                else
                {
                    var cDB = db.Customers.SingleOrDefault(m => m.customerCode == customer.customerCode);
                    cDB.customerCode = customer.customerCode;
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
            Customer c = db.Customers.SingleOrDefault(m => m.customerCode == id);
            if (c != null)
            {
                db.Customers.Remove(c);
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