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
            ViewBag.accounts = db.ExpenseAccounts.ToList();
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
                result = true;
            }
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
            voucherViewModel v = new voucherViewModel();
            v.voucher = new Voucher();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            v.voucher = db.Vouchers.Find(id);
            if (v == null)
            {
                return HttpNotFound();
            }
            return View("ExpenseVouchers", v);
        }
        public ActionResult getExpDetails(int? id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var exp = db.VoucherBodies.Where(r => r.voucherNo == id).ToList();
            return Json(exp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printExpInvoice(int? id)
        {
            voucherViewModel va = new voucherViewModel();
            va.voucher = new Voucher();
            va.voucherDetail = new VoucherBody();
            va.voucher = db.Vouchers.SingleOrDefault(x => x.voucherNo == id);
            ViewBag.vouchers = db.VoucherBodies.Where(x => x.voucherNo == id).ToList();
            return new ViewAsPdf(va);
        }
        public ActionResult Report()
        {
            voucherViewModel vm = new voucherViewModel();
            vm.voucher = new Voucher();
            vm.voucherDetail = new VoucherBody();
            ViewBag.vouchers = (from v in db.Vouchers
                                join vb in db.VoucherBodies on v.voucherNo equals vb.voucherNo
                                select new voucherViewModel
                                {
                                    voucher=v,
                                    voucherDetail=vb,
                                    balance=vb.debit-vb.credit
                                }).ToList();
            return View();
        }
    }
}