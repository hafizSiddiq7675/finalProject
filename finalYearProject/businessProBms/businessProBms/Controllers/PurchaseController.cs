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
            ViewBag.accounts = db.ExpenseAccounts.Where(r => r.isGroup == false && r.isActive == true && r.accountType == "Assets" && r.name == "Cash").ToList();
            purchaseViewModel pvm = new purchaseViewModel();
            pvm.purchases = new Purchase();
            var maxId = db.Purchases.ToList().OrderByDescending(r => r.purchaseId).FirstOrDefault();
            pvm.purchases.purchaseId = maxId == null ? 1 : (maxId.purchaseId) + 1;
            pvm.purchases.purchaseDate = DateTime.Now.Date;
            return View(pvm);
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
            var pv = db.Vouchers.SingleOrDefault(r => r.voucherNo == p.purVoucherNo);
            if (p != null)
            {
                db.Purchases.Remove(p);
                db.Vouchers.Remove(pv);
                db.PurchaseDetails.RemoveRange(db.PurchaseDetails.Where(f => f.purchaseDetailsId == id));
                db.VoucherBodies.RemoveRange(db.VoucherBodies.Where(r => r.voucherNo == pv.voucherNo));
                db.SaveChanges();
                result = true;
            }
            else
            {
                PurchaseDetail pd = db.PurchaseDetails.Single(r => r.code == id);
                var vb = db.VoucherBodies.Where(r => r.voucherNo == pd.Purchase.purVoucherNo);
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
            ViewBag.accounts = db.ExpenseAccounts.Where(r => (r.isGroup == false) && (r.isActive == true) && (r.accountType == "Assets") && (r.name == "Cash")).ToList();
            purchaseViewModel pu = new purchaseViewModel();
            pu.purchases = new Purchase();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var p = db.Purchases.Find(id);
            pu.purchases = p;
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
        [HttpPost]
        public ActionResult voucher(purchaseViewModel pvm)
        {
            Voucher vo = new Voucher();
            var max = db.Vouchers.ToList().OrderByDescending(r => r.voucherNo).FirstOrDefault();
            vo.voucherNo = max == null ? 1 : (max.voucherNo) + 1;
            vo.voucherDate = pvm.purchaseDate;
            vo.voucherType = 10; //Purchase Voucher
            db.Vouchers.Add(vo);
            db.SaveChanges();
            Purchase purchase = new Purchase();
            purchase.purchaseId = pvm.purchaseId;
            purchase.purchaseDate = pvm.purchaseDate;
            purchase.vendorCode = pvm.vendorCode;
            purchase.vendorName = pvm.vendorName;
            purchase.purVoucherNo = vo.voucherNo;
            db.Purchases.Add(purchase);
            decimal cr = 0;
            foreach (var item in pvm.purchaseDetails)
            {
                PurchaseDetail pd = new PurchaseDetail();
                    pd.serialNo = db.Purchases.Sum(r => r.purchaseId).ToString();
                    Product uom = db.Products.Single(s => s.productCode == item.productCode);
                    if (uom != null)
                    {
                        uom.currentQuantity += item.quantity;
                        uom.averagePrice = uom.averagePrice + item.purchasePrice / 2;
                        uom.lastPurchasePrice = item.purchasePrice;
                    }
                    pd.purchaseDetailsId = pvm.purchaseId;
                    pd.productCode = item.productCode;
                    pd.productName = item.productName;
                    pd.unitOfMeasure = item.unitOfMeasure;
                    pd.quantity = item.quantity;
                    pd.purchasePrice = item.purchasePrice;
                    pd.amount = item.amount;
                    db.PurchaseDetails.Add(pd);
                    db.SaveChanges();
                    VoucherBody vbdp = new VoucherBody();
                    ExpenseAccount ex = new ExpenseAccount();
                    ex = db.ExpenseAccounts.Single(r => r.code == uom.chartOfAccCode);
                    vbdp.accountNo = ex.code;
                    vbdp.accountName = ex.name;
                    vbdp.debit = item.amount;
                    vbdp.credit = 0;
                    vbdp.description = "Item Purchased";
                    vbdp.voucherNo = vo.voucherNo;
                    db.VoucherBodies.Add(vbdp);
                    db.SaveChanges();
                    cr += item.amount;
            }
            VoucherBody vbcp = new VoucherBody();
            var ven = db.Vendors.SingleOrDefault(r => r.vendorCode == pvm.vendorCode);
            var ea = db.ExpenseAccounts.Single(r => r.code == ven.chartOfAccCode);
            vbcp.accountNo = ea.code;
            vbcp.accountName = ea.name;
            vbcp.credit = cr;
            vbcp.debit = 0;
            vbcp.description = "Item Purchased";
            vbcp.voucherNo = vo.voucherNo;
            db.VoucherBodies.Add(vbcp);
            db.SaveChanges();
            if (pvm.transactionType == 1)
            {
                Voucher v = new Voucher();
                var maximum = db.Vouchers.ToList().OrderByDescending(r => r.voucherNo).FirstOrDefault();
                v.voucherNo = max == null ? 1 : (maximum.voucherNo) + 1;
                v.voucherDate = pvm.purchaseDate;
                v.voucherType = 1; //Cash Payment
                db.Vouchers.Add(v);
                db.SaveChanges();
                VoucherBody vbd = new VoucherBody();
                var vender = db.Vendors.SingleOrDefault(r => r.vendorCode == pvm.vendorCode);
                var exv = db.ExpenseAccounts.Single(r => r.code == vender.chartOfAccCode);
                vbd.accountNo = exv.code;
                vbd.accountName = exv.name;
                vbd.credit = 0;
                vbd.debit = pvm.netPayment;
                vbd.description = "Cash Payment";
                vbd.voucherNo = v.voucherNo;
                db.VoucherBodies.Add(vbd);
                db.SaveChanges();
                VoucherBody vbc = new VoucherBody();
                ExpenseAccount exp = db.ExpenseAccounts.Single(r => r.code==pvm.accountno);
                vbc.accountNo = exp.code;
                vbc.accountName = exp.name;
                vbc.debit = 0;
                vbc.credit = pvm.netPayment;
                vbc.description = "Paid";
                vbc.voucherNo = v.voucherNo;
                db.VoucherBodies.Add(vbc);
                db.SaveChanges();
            }
            return Json("");
        }
    }
}