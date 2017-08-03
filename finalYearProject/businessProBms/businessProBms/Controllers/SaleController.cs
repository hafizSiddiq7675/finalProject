using System;
using System.Linq;
using System.Web.Mvc;
using businessProBms.Models;
using Rotativa;
namespace businessProBms.Controllers
{
    public class SaleController : Controller
    {
        businessProBmsEntities db = new businessProBmsEntities();
        //
        // GET: /Sales/
        public ActionResult Sales()
        {
            Sale s = new Sale();
            s.saleId = db.Sales.Max(pur => pur.saleId) + 1;
            s.saleDate = DateTime.Now.Date;
            ViewBag.customers = db.Customers.ToList();
            return View(s);
        }
       [HttpPost]
        public ActionResult Add([Bind(Include = "saleId,saleDate,customerCode,customerName")] Sale sale)
        {
            bool result = false;
            if(ModelState.IsValid)
            {
                db.Sales.Add(sale);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getSaleIds()
        {
            var Ids = db.Sales.Select(r => new { value = r.saleId, text = r.saleId });
            return new JsonResult { Data = Ids, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult getProducts()
        {
            var product = db.Products.Select(r => new { value = r.code, text = r.name });
            return new JsonResult { Data = product, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult AddDetails([Bind(Include="saleDetailsId,serialNo,productCode,productName,unitOfMeasure,quantity,salePrice,amount")] SaleDetail saleD)
        {
            bool result = false;
            saleD.amount = saleD.quantity * saleD.salePrice;
            saleD.serialNo = db.Sales.Sum(r => r.saleId).ToString();
            Product uom = db.Products.First(s => s.code == saleD.productCode);
            if(uom!=null)
            {
                saleD.unitOfMeasure = uom.UOM;
            }
            TryUpdateModel(saleD);
            if(ModelState.IsValid)
            {
                db.SaleDetails.Add(saleD);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Sale s = db.Sales.SingleOrDefault(f => f.saleId == id);
            if (s != null)
            {
                db.Sales.Remove(s);
                db.SaleDetails.RemoveRange(db.SaleDetails.Where(f => f.saleDetailsId == id));
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getAllSales()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var sal = db.Sales.ToList();
            return Json(sal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printSaleInvoice(int? id)
        {
            Sale sa = db.Sales.SingleOrDefault(x => x.saleId == id);
            ViewBag.sale = db.SaleDetails.Where(x => x.saleDetailsId == id).ToList();
            return new ViewAsPdf(sa);
        }
        public ActionResult Edit(int? id)
        {
            return View("Purchases");
        }
	}
}