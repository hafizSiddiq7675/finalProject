using System;
using System.Linq;
using System.Web.Mvc;
using businessProBms.Models;
using Rotativa;
using System.Net;
namespace businessProBms.Controllers
{
    public class SaleController : Controller
    {
        businessProBmsEntities db = new businessProBmsEntities();
        //
        // GET: /Sales/
        public ActionResult Sales()
        {
            saleViewModel svm = new saleViewModel();
            svm.sales = new Sale();
            svm.saleDetails = new SaleDetail();
            svm.sales.saleId = db.Sales.Max(pur => pur.saleId) + 1;
            svm.sales.saleDate = DateTime.Now.Date;
            ViewBag.customers = db.Customers.ToList();
            ViewBag.products = db.Products.ToList();
            ViewBag.sales = db.Sales.ToList();
            return View(svm);
        }
        [HttpPost]
        public JsonResult Sales(saleViewModel sa)
            {
                bool result = false;
            if (sa != null)
            {
                int count = db.Sales.Count(r => r.saleId == sa.saleId);
                if (count == 0)
                {
                    Sale sale = new Sale();
                    sale.saleId = sa.saleId;
                    sale.saleDate = sa.saleDate;
                    sale.customerCode = sa.customerCode;
                    sale.customerName = sa.customerName;
                    db.Sales.Add(sale);
                    db.SaveChanges();
                    SaleDetail saledetail = new SaleDetail();
                    saledetail.serialNo = db.Sales.Sum(r => r.saleId).ToString();
                    Product uom = db.Products.Single(s => s.code == sa.productCode);
                    if (uom != null)
                    {
                        saledetail.unitOfMeasure = uom.UOM;
                    }
                    saledetail.saleDetailsId = sa.saleId;
                    saledetail.productCode = sa.productCode;
                    saledetail.productName = sa.productName;
                    saledetail.quantity = sa.quantity;
                    saledetail.salePrice = sa.salePrice;
                    saledetail.amount = sa.quantity * sa.salePrice;
                    db.SaleDetails.Add(saledetail);
                    db.SaveChanges();
                    result = true;
                }
                else
                {
                    SaleDetail saledetail = new SaleDetail();
                    saledetail.serialNo = db.Sales.Sum(r => r.saleId).ToString();
                    Product uom = db.Products.Single(s => s.code == sa.productCode);
                    if (uom != null)
                    {
                        saledetail.unitOfMeasure = uom.UOM;
                    }
                    saledetail.saleDetailsId = sa.saleId;
                    saledetail.productCode = sa.productCode;
                    saledetail.productName = sa.productName;
                    saledetail.quantity = sa.quantity;
                    saledetail.salePrice = sa.salePrice;
                    saledetail.amount = sa.quantity * sa.salePrice;
                    db.SaleDetails.Add(saledetail);
                    db.SaveChanges();
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSaleDetails(int? id)
        {
            decimal total = 0;
            db.Configuration.ProxyCreationEnabled = false;
            total = db.SaleDetails.Where(r => r.saleDetailsId == id).Sum(r => r.amount);
            var sales = db.SaleDetails.Where(r => r.saleDetailsId == id);
            var result = new { Sales = sales, Total = total };
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
            else
            {
                SaleDetail sd = db.SaleDetails.Single(r => r.code == id);
                db.SaleDetails.Remove(sd);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printSaleInvoice(int? id)
        {
            int total = 0;
            Sale sa = db.Sales.SingleOrDefault(x => x.saleId == id);
            ViewBag.sale = db.SaleDetails.Where(x => x.saleDetailsId == id).ToList();
            total = (int)db.SaleDetails.Where(r => r.saleDetailsId == id).Sum(c => c.amount);
            convertToNumeral convert = new convertToNumeral();
            ViewBag.total = convert.convertNumber(total);
            return new ViewAsPdf(sa);  
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.customers = db.Customers.ToList();
            ViewBag.products = db.Products.ToList();
            ViewBag.sales = db.Sales.ToList();
            saleViewModel sa = new saleViewModel();
            sa.sales = new Sale();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sa.sales = db.Sales.Find(id);
            if (sa == null)
            {
                return HttpNotFound();
            }
            return View("Sales", sa);
        }
	}
}