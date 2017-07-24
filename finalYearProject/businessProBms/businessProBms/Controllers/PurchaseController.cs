using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using businessProBms.Models;

namespace businessProBms.Controllers
{
    public class PurchaseController : Controller
    {
        //
        // GET: /Purchase/
        businessProBmsEntities db = new businessProBmsEntities();
        public ActionResult Purchases()
        {
            ViewBag.vendors = db.Vendors.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchases([Bind(Include="purchaseId,purchaseDate,vendorCode,vendorName")] Purchase purchase)
        {
            if(ModelState.IsValid)
            {
                var pur = db.Purchases.SingleOrDefault(m => m.purchaseId == purchase.purchaseId);
                    if(pur==null)
                    {
                        db.Purchases.Add(purchase);
                    }
                    else
                    {
                        var purDb=db.Purchases.Single(m=>m.purchaseId==pur.purchaseId);
                        purDb.purchaseId=purchase.purchaseId;
                        purDb.purchaseDate=purchase.purchaseDate;
                        purDb.vendorCode=purchase.vendorCode;
                        purDb.vendorName=purchase.vendorName;
                    }
                db.SaveChanges();
                return RedirectToAction("Purchases");
            }
            ViewBag.vendors=db.Vendors.ToList();
            return View(purchase);
        }
	}
}