using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using businessProBms.Models;
using System.Net;
using Rotativa;
namespace businessProBms.Controllers
{
    public class ExpenseVoucherController : Controller
    {
        businessProBmsEntities db = new businessProBmsEntities();

        // GET: /ExpenseVoucher/ExpenseVouchers
        public ActionResult ExpenseVouchers()
        {
            voucherViewModel vm = new voucherViewModel();
            vm.voucher = new Voucher();
            vm.voucher.voucherNo = db.Vouchers.Max(x => x.voucherNo) + 1;
            vm.voucher.voucherDate = DateTime.Now.Date;
            ViewBag.vouchers = db.Vouchers.ToList();
            ViewBag.accounts = db.ExpenseAccounts.Where(r=>r.isGroup==false).ToList();
            var enumData = from VoucherTypeModel v in Enum.GetValues(typeof(VoucherTypeModel))
                           select new
                           {
                               value = v.ToString(),
                               text = (int)v
                           };
            ViewBag.enumList = new SelectList(enumData, "text", "value");
            return View(vm);
        }
        [HttpPost]
        public JsonResult ExpenseVouchers(voucherLine vm)
        {
            var result = false;
            if (vm != null)
            {
                int count = db.Vouchers.Count(o => o.voucherNo == vm.voucherNo);
                Voucher vr = db.Vouchers.Find(vm.voucherNo);
                    if (count == 0)
                    {
                        //post data to both tables
                        Voucher v = new Voucher();
                        v.voucherNo = vm.voucherNo;
                        v.voucherDate = vm.voucherDate;
                        v.voucherType = vm.voucherType;
                        db.Vouchers.Add(v);
                        db.SaveChanges();
                        VoucherBody vb = new VoucherBody();
                        vb.voucherNo = vm.voucherNo;
                        vb.accountNo = vm.accountNo;
                        vb.accountName = vm.accountName;
                        vb.description = vm.description;
                        vb.debit = vm.debit;
                        vb.credit = vm.credit;
                        db.VoucherBodies.Add(vb);
                        db.SaveChanges();
                    }
                    else
                    {
                        //add only to 2nd table
                        VoucherBody vb = new VoucherBody();
                        vb.voucherNo = vm.voucherNo;
                        vb.accountNo = vm.accountNo;
                        vb.accountName = vm.accountName;
                        vb.description = vm.description;
                        vb.debit = vm.debit;
                        vb.credit = vm.credit;
                        db.VoucherBodies.Add(vb);
                        db.SaveChanges();
                    }
                }
                result = true;
            return Json(result, JsonRequestBehavior.AllowGet);   
        }
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Voucher v = db.Vouchers.SingleOrDefault(f => f.voucherNo == id);
            VoucherBody vb = db.VoucherBodies.SingleOrDefault(f => f.code == id);
            if (v != null)
            {
                db.Vouchers.Remove(v);
                db.VoucherBodies.RemoveRange(db.VoucherBodies.Where(f => f.voucherNo == id));
                db.SaveChanges();
                result = true;
            }
            else
            {
                db.VoucherBodies.Remove(vb);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int? id)
        {
            ViewBag.vouchers = db.Vouchers.ToList();
            ViewBag.accounts = db.ExpenseAccounts.ToList();
            var enumData = from VoucherTypeModel v in Enum.GetValues(typeof(VoucherTypeModel))
                           select new
                           {
                               value = v.ToString(),
                               text = (int)v
                           };
            ViewBag.enumList = new SelectList(enumData, "text", "value");
            voucherViewModel vvm = new voucherViewModel();
            vvm.voucher = new Voucher();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vvm.voucher = db.Vouchers.Find(id);
            if (vvm == null)
            {
                return HttpNotFound();
            }
            return View("ExpenseVouchers", vvm);
        }
        public ActionResult getExpDetails(int? id)
        {
            decimal drtot = 0;
            decimal crtot = 0;
            db.Configuration.ProxyCreationEnabled = false;
            var exp = db.VoucherBodies.Where(r => r.voucherNo == id).ToList();
            drtot = db.VoucherBodies.Where(r => r.voucherNo == id).Sum(c => c.debit);
            crtot = db.VoucherBodies.Where(r => r.voucherNo == id).Sum(c => c.credit);
            var result = new { DrTot = drtot, CrTot = crtot, Exp = exp };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printExpInvoice(int? id)
        {
            int total=0;
            voucherViewModel va = new voucherViewModel();
            va.voucher = new Voucher();
            va.voucherDetail = new VoucherBody();
            va.voucher = db.Vouchers.SingleOrDefault(x => x.voucherNo == id);
            ViewBag.vouchers = db.VoucherBodies.Where(x => x.voucherNo == id).ToList();
            total =(int)db.VoucherBodies.Where(r => r.voucherNo == id).Sum(c => c.debit);
            ViewBag.total = total;
            convertToNumeral convert = new convertToNumeral();
            ViewBag.totalInWord = convert.convertNumber(total);
            return new ViewAsPdf(va);
        }
        public ActionResult Report()
        {
            ViewBag.accounts = db.ExpenseAccounts.Where(r => r.isGroup == false).ToList();
            return View();
        }
        public JsonResult srcData(int acc, DateTime startDate, DateTime endDate)
        {
            var data = (from v in db.Vouchers
                        join vb in db.VoucherBodies on v.voucherNo equals vb.voucherNo
                        where (vb.accountNo == acc) && (v.voucherDate >= startDate) && (v.voucherDate <= endDate)
                        select new
                        {
                            vdate = v.voucherDate,
                            vno = v.voucherNo,
                            accountno = vb.accountNo,
                            accountname = vb.accountName,
                            debit = vb.debit,
                            credit = vb.credit,
                        }).ToList();
            decimal total = 0;
            decimal currentTot=0;
            var item = data.OrderBy(i => i.vdate).Select(r =>
            {
                currentTot += (r.debit-r.credit);
                total += currentTot;
                return new { vdate = r.vdate, vno = r.vno, accountno = r.accountno, accountname = r.accountname, debit = r.debit, credit = r.credit, balance = currentTot, total};
            });
            var result = new { Total = total, Result = item };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}