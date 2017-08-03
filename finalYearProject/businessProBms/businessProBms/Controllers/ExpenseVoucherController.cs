using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using businessProBms.Models;
namespace businessProBms.Controllers
{
    public class ExpenseVoucherController : Controller
    {
        businessProBmsEntities db = new businessProBmsEntities();
        //
        // GET: /ExpenseVoucher/ExpenseVouchers
        public ActionResult ExpenseVouchers()
        {
            voucherViewModel vm = new voucherViewModel();
            //vm.voucher.voucherNo = db.Vouchers.Max(x => x.voucherNo) + 1;
            //vm.voucher.voucherDate = DateTime.Now.Date;
            ViewBag.vouchers = db.Vouchers.ToList();
            ViewBag.accounts = db.ExpenseAccounts.ToList();
            return View();
        }
	}
}