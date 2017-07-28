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
        public ActionResult Add([Bind(Include="purchaseId, purchaseDate,vendorCode,vendorName")] Purchase pur)
        {
            bool result=false;
            if(ModelState.IsValid)
            {
                db.Purchases.Add(pur);
                db.SaveChanges();
                result=true;
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddDetails([Bind(Include = "purchaseDetailsId,serialNo,productCode,productName,unitOfMeasure,quantity,purchasePrice")] PurchaseDetail purD)
        {
            bool result = false;
            purD.serialNo = db.Purchases.Sum(r => r.purchaseId).ToString();
            Product uom = db.Products.First(s => s.code == purD.productCode);
            if (uom != null)
            {
                purD.unitOfMeasure = uom.UOM;
            }
            TryUpdateModel(purD);
            if(ModelState.IsValid)
            {

                db.PurchaseDetails.Add(purD);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPurchaseIds()      
        
        {
            var Ids = db.Purchases.Select(r => new { value = r.purchaseId, text = r.purchaseId });
            return new JsonResult { Data = Ids, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult getProducts()
        {
            var product = db.Products.Select(r => new { value = r.code, text = r.name });
            return new JsonResult { Data = product, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
	}
}