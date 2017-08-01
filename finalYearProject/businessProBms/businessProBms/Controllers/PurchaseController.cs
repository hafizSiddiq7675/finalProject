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
            Purchase p = new Purchase();
            p.purchaseId = db.Purchases.Max(pur => pur.purchaseId) + 1;
            p.purchaseDate = DateTime.Now.Date;
            ViewBag.purchase = db.Purchases.ToList();
            ViewBag.vendors = db.Vendors.ToList();
            return View(p);
        }
        [HttpPost]
        public ActionResult Add([Bind(Include="purchaseId, purchaseDate,vendorCode,vendorName")] Purchase pur)
        {
            bool result=false;
            if(ModelState.IsValid)
            {
                Purchase pr = db.Purchases.SingleOrDefault(s => s.purchaseId == pur.purchaseId);
                if(pr==null)
                {
                    db.Purchases.Add(pur);
                }
                else
                {
                    var prD = db.Purchases.Single(s => s.purchaseId == pur.purchaseId);
                    prD.purchaseId = pur.purchaseId;
                    prD.purchaseDate = pur.purchaseDate;
                    prD.vendorCode = pur.vendorCode;
                    prD.vendorName = pur.vendorName;
                }
                db.SaveChanges();
                result=true;
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddDetails([Bind(Include = "purchaseDetailsId,serialNo,productCode,productName,unitOfMeasure,quantity,purchasePrice")] PurchaseDetail purD)
        {
            bool result = false;
            purD.serialNo = db.Purchases.Sum(s => s.purchaseId).ToString();
            purD.amount = purD.quantity * purD.purchasePrice;
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
        [HttpPost]
        public ActionResult AddEditedDetails([Bind(Include = "code,purchaseDetailsId,serialNo,productCode,productName,unitOfMeasure,quantity,purchasePrice,amount")] PurchaseDetail purD)
        {
            bool result = false;
            if(ModelState.IsValid)
            {
                var pr = db.PurchaseDetails.Single(s => s.code == purD.code);
                Product uom = db.Products.First(s => s.code == purD.code);
                if (uom != null)
                {
                    purD.unitOfMeasure = uom.UOM;
                }
                //TryUpdateModel(purD);
                pr.code = purD.code;
                pr.purchaseDetailsId = purD.purchaseDetailsId;
                pr.serialNo = purD.serialNo;
                pr.productCode = purD.productCode;
                pr.productName = purD.productName;
                pr.unitOfMeasure = purD.unitOfMeasure;
                pr.quantity = purD.quantity;
                pr.purchasePrice = purD.purchasePrice;
                pr.amount = purD.amount;
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
        public ActionResult showAll()
        {
            ViewBag.purchase = db.Purchases.ToList();
            ViewBag.vendors = db.Vendors.ToList();
            return View();
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Purchase pr = db.Purchases.SingleOrDefault(s => s.purchaseId == id);
            if (pr != null)
            {
                db.Purchases.Remove(pr);
                db.PurchaseDetails.RemoveRange(db.PurchaseDetails.Where(s => s.purchaseDetailsId == id));
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult detailDeleteConfirmed(int? id)
        {
            bool result = false;
            PurchaseDetail pur = db.PurchaseDetails.SingleOrDefault(s => s.code == id);
            if(pur!=null)
            {
                db.PurchaseDetails.Remove(pur);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPurchaseDetail(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            PurchaseDetail pur = db.PurchaseDetails.ToList().Find(s => s.code == id);
            return Json(pur, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPurchase(int id)
        {
            //For Avoidinig a Circuler Loop
            db.Configuration.ProxyCreationEnabled = false;
            Purchase pr = db.Purchases.ToList().Find(s => s.purchaseId==id);
            return Json(pr, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPurchaseDetails(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.PurchaseDetails.Where(s => s.purchaseDetailsId == id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}