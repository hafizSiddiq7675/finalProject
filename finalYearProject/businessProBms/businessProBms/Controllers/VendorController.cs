using System.Linq;
using System.Web.Mvc;
using businessProBms.Models;
using PagedList;
using System.Net;
using System;
namespace businessProBms.Controllers
{
    public class VendorController : Controller
    {
        private businessProBmsEntities db = new businessProBmsEntities();
        //
        // GET: /Vendor/Vendors
        public ActionResult Vendors(int? page)
        {
            Vendor v = new Vendor();
            var maxId = db.Vendors.ToList().OrderByDescending(r => r.vendorCode).FirstOrDefault();
            v.vendorCode = maxId == null ? 1 : (maxId.vendorCode) + 1;
            ViewBag.vendors = (db.Vendors.ToList().ToPagedList(page?? 1,8));
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vendors([Bind(Include="vendorCode,name,contact,address,debitLimit,chartOfAccCode, openingCredit")] Vendor vendor)
        {
            if(ModelState.IsValid)
            {
                Vendor v = db.Vendors.Find(vendor.vendorCode);
                ExpenseAccount ex = new ExpenseAccount();
                if(v==null)
                {
                    var maxCode = db.ExpenseAccounts.ToList().OrderByDescending(r => r.code).FirstOrDefault();
                    ex.code = maxCode == null ? 1 : (maxCode.code) + 1;
                    ex.accountType = "Liability";
                    ex.description = "Vendors Accounts";
                    ex.name = vendor.name + " Payables";
                    ex.parentCode = 5;
                    ex.isGroup = false;
                    ex.isActive = true;
                    ex.openingDebit = 0;
                    ex.openingCredit = vendor.openingCredit;
                    db.ExpenseAccounts.Add(ex);
                    db.SaveChanges();
                    vendor.chartOfAccCode = ex.code;
                    db.Vendors.Add(vendor);
                }
                else
                {
                    var vDb = db.Vendors.SingleOrDefault(m => m.vendorCode == vendor.vendorCode);
                    var eDb = db.ExpenseAccounts.SingleOrDefault(m => m.code == vendor.chartOfAccCode);
                    vDb.vendorCode = vendor.vendorCode;
                    vDb.name = vendor.name;
                    vDb.contact = vendor.contact;
                    vDb.address = vendor.address;
                    vDb.debitLimit = vendor.debitLimit;
                    eDb.name = vendor.name;
                    eDb.openingCredit = vendor.openingCredit;
                }
                db.SaveChanges();
                return RedirectToAction("Vendors");
            }
            return View(vendor);
        }
        public ActionResult Edit(int? id, int?page)
        {
            ViewBag.vendors = (db.Vendors.ToList().ToPagedList(page ?? 1, 8));
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor v = db.Vendors.Find(id);
            var exp = db.ExpenseAccounts.SingleOrDefault(m => m.code == v.chartOfAccCode);
            v.openingCredit = exp.openingCredit;
            if(v==null)
            {
                return HttpNotFound();
            }
            return View("Vendors", v);
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Vendor v = db.Vendors.SingleOrDefault(m => m.vendorCode == id);
            var exp = db.ExpenseAccounts.SingleOrDefault(m => m.code == v.chartOfAccCode);
            if(v!=null)
            {
                exp.isActive = false;
                db.Vendors.Remove(v);
                db.SaveChanges();
                result=true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}