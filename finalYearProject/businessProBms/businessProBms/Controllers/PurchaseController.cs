using System;
using System.Linq;
using System.Web.Mvc;
using businessProBms.Models;
using Rotativa;
using System.Net;
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
            ViewBag.products = db.Products.ToList();
            ViewBag.purchases = db.Purchases.ToList();
            purchaseViewModel pvm = new purchaseViewModel();
            pvm.purchases = new Purchase();
            pvm.purchaseDetails = new PurchaseDetail();
            var maxId = db.Purchases.ToList().OrderByDescending(r => r.purchaseId).FirstOrDefault();
            pvm.purchases.purchaseId = maxId == null ? 1 : (maxId.purchaseId) + 1;
            pvm.purchases.purchaseDate = DateTime.Now.Date;
            return View(pvm);
        }
        [HttpPost]
        public JsonResult Purchases(purchaseViewModel pu)
        {
            bool result = false;
            if (pu != null)
            {
                int count = db.Purchases.Count(r => r.purchaseId == pu.purchaseId);
                if (count == 0)
                {
                    Purchase purchase = new Purchase();
                    purchase.purchaseId = pu.purchaseId;
                    purchase.purchaseDate = pu.purchaseDate;
                    purchase.vendorCode = pu.vendorCode;
                    purchase.vendorName = pu.vendorName;
                    db.Purchases.Add(purchase);
                    db.SaveChanges();
                    Voucher v = new Voucher();
                    var max = db.Vouchers.ToList().OrderByDescending(r => r.voucherNo).FirstOrDefault();
                    v.voucherNo = max == null ? 1 : (max.voucherNo) + 1;
                    v.voucherDate = pu.purchaseDate;
                    v.voucherType = 10;
                    db.Vouchers.Add(v);
                    db.SaveChanges();
                    PurchaseDetail purchasedetail = new PurchaseDetail();
                    purchasedetail.serialNo = db.Purchases.Sum(r => r.purchaseId).ToString();
                    Product uom = db.Products.Single(s => s.productCode == pu.productCode);
                    if (uom != null)
                    {
                        purchasedetail.unitOfMeasure = uom.UOM;
                        uom.currentQuantity += pu.quantity;
                        uom.lastPurchasePrice = pu.purchasePrice;
                    }
                    purchasedetail.purchaseDetailsId = pu.purchaseId;
                    purchasedetail.productCode = pu.productCode;
                    purchasedetail.productName = pu.productName;
                    purchasedetail.quantity = pu.quantity;
                    purchasedetail.purchasePrice = pu.purchasePrice;
                    purchasedetail.amount = pu.purchasePrice * pu.quantity;
                    db.PurchaseDetails.Add(purchasedetail);
                    db.SaveChanges();
                    VoucherBody vbc = new VoucherBody();
                    var ven = db.Vendors.SingleOrDefault(r => r.vendorCode == pu.vendorCode);
                    vbc.accountNo = ven.chartOfAccCode;
                    vbc.accountName = ven.name;
                    vbc.credit = pu.quantity * pu.purchasePrice;
                    vbc.debit = 0;
                    vbc.description = "Item Purchased";
                    vbc.voucherNo = db.Vouchers.Max(r => r.voucherNo);
                    db.VoucherBodies.Add(vbc);
                    db.SaveChanges();
                    VoucherBody vbd = new VoucherBody();
                    ExpenseAccount ex = new ExpenseAccount();
                    ex = db.ExpenseAccounts.Single(r => r.code == uom.chartOfAccCode);
                    vbd.accountNo = ex.code;
                    vbd.accountName = ex.name;
                    vbd.debit = pu.quantity * pu.purchasePrice;
                    vbd.credit = 0;
                    vbd.description = "Item Purchased";
                    vbd.voucherNo = db.Vouchers.Max(r => r.voucherNo);
                    db.VoucherBodies.Add(vbd);
                    db.SaveChanges();
                    result = true;
                }
                else
                {
                    PurchaseDetail purchasedetail = new PurchaseDetail();
                    purchasedetail.serialNo = db.Purchases.Sum(r => r.purchaseId).ToString();
                    Product uom = db.Products.Single(s => s.productCode == pu.productCode);
                    if (uom != null)
                    {
                        purchasedetail.unitOfMeasure = uom.UOM;
                        uom.currentQuantity += pu.quantity;
                        uom.lastPurchasePrice = pu.purchasePrice;
                    }
                    purchasedetail.purchaseDetailsId = pu.purchaseId;
                    purchasedetail.productCode = pu.productCode;
                    purchasedetail.productName = pu.productName;
                    purchasedetail.quantity = pu.quantity;
                    purchasedetail.purchasePrice = pu.purchasePrice;
                    purchasedetail.amount = pu.purchasePrice * pu.quantity;
                    db.PurchaseDetails.Add(purchasedetail);
                    db.SaveChanges();
                    VoucherBody vbc = new VoucherBody();
                    var ven = db.Vendors.SingleOrDefault(r => r.vendorCode == pu.vendorCode);
                    vbc.accountNo = ven.chartOfAccCode;
                    vbc.accountName = ven.name;
                    vbc.credit = pu.quantity * pu.purchasePrice;
                    vbc.debit = 0;
                    vbc.description = "Item Purchased";
                    vbc.voucherNo = db.Vouchers.Max(r => r.voucherNo);
                    db.VoucherBodies.Add(vbc);
                    db.SaveChanges();
                    VoucherBody vbd = new VoucherBody();
                    ExpenseAccount ex = new ExpenseAccount();
                    ex = db.ExpenseAccounts.Single(r => r.code == uom.chartOfAccCode);
                    vbd.accountNo = ex.code;
                    vbd.accountName = ex.name;
                    vbd.debit = pu.quantity * pu.purchasePrice;
                    vbd.credit = 0;
                    vbd.description = "Item Purchased";
                    vbd.voucherNo = db.Vouchers.Max(r => r.voucherNo);
                    db.VoucherBodies.Add(vbd);
                    db.SaveChanges();
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPurchaseDetails(int? id)
        {
            decimal total = 0;
            db.Configuration.ProxyCreationEnabled = false;
            total = db.PurchaseDetails.Where(r => r.purchaseDetailsId == id).Sum(r => r.amount);
            var purchase = from p in db.PurchaseDetails
                           where (p.purchaseDetailsId == id)
                           join pr in db.Products on p.productCode equals pr.productCode
                           select p;
            var result = new { Purchase = purchase, Total = total };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Purchase p = db.Purchases.SingleOrDefault(f => f.purchaseId == id);
            if (p != null)
            {
                db.Purchases.Remove(p);
                db.PurchaseDetails.RemoveRange(db.PurchaseDetails.Where(f => f.purchaseDetailsId == id));
                db.SaveChanges();
                result = true;
            }
            else
            {
                PurchaseDetail pd = db.PurchaseDetails.Single(r => r.code == id);
                db.PurchaseDetails.Remove(pd);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.vendors = db.Vendors.ToList();
            ViewBag.products = db.Products.ToList();
            ViewBag.purchases = db.Purchases.ToList();
            purchaseViewModel pu = new purchaseViewModel();
            pu.purchases = new Purchase();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pu.purchases = db.Purchases.Find(id);
            if (pu == null)
            {
                return HttpNotFound();
            }
            return View("Purchases", pu);
        }
        public ActionResult printPurchaseInvoice(int id)
        {
            int total = 0;
            Purchase pa = db.Purchases.SingleOrDefault(x => x.purchaseId == id);
            ViewBag.purchase = db.PurchaseDetails.Where(x => x.purchaseDetailsId == id).ToList();
            total = (int)db.PurchaseDetails.Where(r => r.purchaseDetailsId == id).Sum(c => c.amount);
            convertToNumeral convert= new convertToNumeral();
            ViewBag.total = convert.convertNumber(total);
            return new ViewAsPdf(pa);
        }
        public JsonResult getProduct(int? id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Product p = db.Products.Single(r => r.productCode == id);
            return Json(p, JsonRequestBehavior.AllowGet);
        }
    }
}