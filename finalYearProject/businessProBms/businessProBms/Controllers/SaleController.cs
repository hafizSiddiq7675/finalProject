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
            var MaxId = db.Sales.ToList().OrderByDescending(r => r.saleId).FirstOrDefault();
            svm.sales.saleId = MaxId == null ? 1 : (MaxId.saleId) + 1;
            svm.sales.saleDate = DateTime.Now.Date;
            ViewBag.customers = db.Customers.ToList();
            ViewBag.products = db.Products.ToList();
            ViewBag.sales = db.Sales.ToList();
            ViewBag.accounts = db.ExpenseAccounts.Where(r => (r.isGroup == false) && (r.isActive == true) && (r.accountType == "Assets") && (r.name == "Cash")).ToList();
            return View(svm);
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
        public JsonResult getProduct(int? id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Product p = db.Products.Single(r => r.productCode == id);
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult voucher(saleViewModel svm)
        {
            Voucher vo = new Voucher();
            var max = db.Vouchers.ToList().OrderByDescending(r => r.voucherNo).FirstOrDefault();
            vo.voucherNo = max == null ? 1 : (max.voucherNo) + 1;
            vo.voucherDate = svm.saleDate;
            vo.voucherType = 11; //Sale Voucher
            db.Vouchers.Add(vo);
            db.SaveChanges();
            Sale sale = new Sale();
            sale.saleId = svm.saleId;
            sale.saleDate = svm.saleDate;
            sale.customerCode = svm.customerCode;
            sale.customerName = svm.customerName;
            sale.saleVoucherNo = vo.voucherNo;
            db.Sales.Add(sale);
            db.SaveChanges();
            decimal dr = 0;
            foreach (var item in svm.saleDetails)
            {
                SaleDetail sd = new SaleDetail();
                sd.serialNo = db.Sales.Select(r => r.saleId).Sum().ToString();
                Product uom = db.Products.Single(r => r.productCode == item.productCode);
                if (uom != null)
                {
                    if (uom.currentQuantity > 0)
                    {
                        uom.currentQuantity -= item.quantity;
                    }
                    uom.price = item.salePrice;
                }
                sd.saleDetailsId = svm.saleId;
                sd.productCode = item.productCode;
                sd.productName = item.productName;
                sd.unitOfMeasure = item.unitOfMeasure;
                sd.quantity = item.quantity;
                sd.salePrice = item.salePrice;
                sd.amount = item.amount;
                db.SaleDetails.Add(sd);
                db.SaveChanges();
                VoucherBody vbcp = new VoucherBody();
                ExpenseAccount ea = db.ExpenseAccounts.Single(r => r.code == uom.chartOfAccCode + 1);
                vbcp.accountNo = ea.code;
                vbcp.accountName = ea.name;
                vbcp.credit = item.amount;
                vbcp.debit = 0;
                vbcp.description = "Item Sold";
                vbcp.voucherNo = vo.voucherNo;
                dr += item.amount;
            }
            VoucherBody vbdp = new VoucherBody();
            var cus = db.Customers.SingleOrDefault(r => r.customerCode == svm.customerCode);
            var ex=db.ExpenseAccounts.Single(r=>r.code==cus.chartOfAccCode);
            vbdp.accountNo = ex.code;
            vbdp.accountName = ex.name;
            vbdp.credit = 0;
            vbdp.debit = dr;
            vbdp.description = "Item Sold";
            vbdp.voucherNo = vo.voucherNo;
            db.VoucherBodies.Add(vbdp);
            db.SaveChanges();
            if (svm.transactionType == 1)
            {
                Voucher v = new Voucher();
                var maximum = db.Vouchers.ToList().OrderByDescending(r => r.voucherNo).FirstOrDefault();
                v.voucherNo = maximum == null ? 1 : (maximum.voucherNo) + 1;
                v.voucherDate = svm.saleDate;
                v.voucherType = 2; //Cash Reciept
                db.Vouchers.Add(v);
                db.SaveChanges();
                VoucherBody vbd = new VoucherBody();
                var exp = db.ExpenseAccounts.Single(r => r.code == svm.accountno);
                vbd.accountNo = exp.code;
                vbd.accountName = exp.name;
                vbd.debit = svm.netPayment;
                vbd.credit = 0;
                vbd.description = "Cash Received";
                vbd.voucherNo = v.voucherNo;
                db.VoucherBodies.Add(vbd);
                db.SaveChanges();
                VoucherBody vbc = new VoucherBody();
                var customer = db.Customers.SingleOrDefault(r => r.customerCode == svm.customerCode);
                var exc = db.ExpenseAccounts.Single(r => r.code == customer.chartOfAccCode);
                vbc.accountNo = exc.code;
                vbc.accountName = exc.name;
                vbc.credit = svm.netPayment;
                vbc.debit = 0;
                vbc.description = "net Payment";
                vbc.voucherNo = v.voucherNo;
                db.VoucherBodies.Add(vbc);
                db.SaveChanges();
            }
            return Json("");
        }
	}
}