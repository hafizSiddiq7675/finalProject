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
            var maxId = db.Vouchers.ToList().OrderByDescending(r => r.voucherNo).FirstOrDefault();
            vm.voucher.voucherNo = maxId == null ? 1 : (maxId.voucherNo) + 1;
            vm.voucher.voucherDate = DateTime.Now.Date;
            ViewBag.vouchers = db.Vouchers.ToList();
            ViewBag.accounts = db.ExpenseAccounts.Where(r=>r.isGroup==false && r.isActive==true).ToList();
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
            ExpenseAccount ea = db.ExpenseAccounts.Where(a => a.code == acc).SingleOrDefault();
            decimal openingbalance = ea.openingDebit - ea.openingCredit;

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
                            credit = vb.credit
                        }).ToList();
            var preveous = (from v in db.Vouchers
                            join vb in db.VoucherBodies on v.voucherNo equals vb.voucherNo
                            where (vb.accountNo == acc) && (v.voucherDate < startDate)
                            select new
                            {
                                debit = vb.debit,
                                credit = vb.credit
                            }).ToList();
            decimal prebalance = 0;
            foreach (var v in preveous)
            {
                prebalance += (v.debit - v.credit);
            }
            data.Insert(0, new
            {
                vdate = startDate,
                vno = 0,
                accountno = acc,
                accountname = db.ExpenseAccounts.Where(a => a.code == acc).SingleOrDefault().name + " (Opning Balance)",
                debit = openingbalance >= 0 ? Math.Abs(openingbalance) : 0,
                credit = openingbalance < 0 ? Math.Abs(openingbalance) : 0
            });
            data.Insert(1, new
            {
                vdate = startDate,
                vno = 0,
                accountno = acc,
                accountname = db.ExpenseAccounts.Where(a => a.code == acc).SingleOrDefault().name + " (Preveous Balance)",
                debit = prebalance >= 0 ? Math.Abs(prebalance) : 0,
                credit = prebalance < 0 ? Math.Abs(prebalance) : 0
            });
            decimal drTot = 0;
            decimal crTot = 0;
            decimal currentTot = 0;
            var item = data.OrderBy(i => i.vdate).Select(r =>
            {
                currentTot += (r.debit - r.credit);
                drTot += r.debit;
                crTot += r.credit;
                return new {vdate = r.vdate, vno = r.vno, accountno = r.accountno, accountname = r.accountname, debit = r.debit, credit = r.credit, balance = currentTot, drTot,crTot};
            });
            var result = new { DrTotal = drTot, CrTotal=crTot, Result = item};
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult trailBalance()
        {
            return View();
        }
        public JsonResult Result(DateTime startDate, DateTime endDate)
        {
            var data = (from c in db.ExpenseAccounts where (c.isGroup == false) && (c.isActive == true)
                        join vb in db.VoucherBodies on c.code equals vb.accountNo
                        join v in db.Vouchers on vb.voucherNo equals v.voucherNo
                        where (v.voucherDate >= startDate) && (v.voucherDate <= endDate) 
                        select new
                        {
                            accountname = vb.accountName,
                            debit=vb.debit,
                            credit=vb.credit
                        }).ToList();
            var opening = db.ExpenseAccounts.Where(r=>r.isGroup==false && r.isActive==true && (Math.Abs(r.openingDebit-r.openingCredit))>0).ToList();
            var preveous = (from e in db.ExpenseAccounts
                            where (e.isActive == true) && (e.isGroup == false)
                            join vb in db.VoucherBodies on e.code equals vb.accountNo
                            join v in db.Vouchers on vb.voucherNo equals v.voucherNo
                            where (v.voucherDate < startDate)
                            select new
                            {
                                accountname = vb.accountName,
                                debit = vb.debit,
                                credit = vb.credit
                            }).ToList();
            foreach (var j in opening)
            {
                data.Insert(0, new { accountname = j.name + " Opening", debit = (j.openingDebit - j.openingCredit) >= 0 ? Math.Abs((j.openingDebit - j.openingCredit)) : 0, credit = (j.openingDebit - j.openingCredit) < 0 ? ((j.openingDebit - j.openingCredit)) : 0 });
            }
            foreach (var i in preveous)
            {
                data.Insert(1, new { accountname = i.accountname + " Prevoeous", debit = i.debit, credit = i.credit });
            }
            var item= data.GroupBy(r=>new {r.accountname}).Select(s=>{
                return new {balance=(s.Sum(c=>c.debit)-s.Sum(c=>c.credit)), accountname=s.Select(c=>c.accountname).Distinct()};
            });
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult incomeStatement()
        {
            return View();
        }
        public JsonResult income(DateTime startDate, DateTime endDate)
        {
            var data = (from e in db.ExpenseAccounts
                           where (e.accountType == "Revenue" || e.accountType=="Expense") && (e.isGroup == false) && (e.isActive==true)
                           join vb in db.VoucherBodies on e.code equals vb.accountNo
                           join v in db.Vouchers on vb.voucherNo equals v.voucherNo where (v.voucherDate>=startDate) && (v.voucherDate<=endDate)
                           select new
                           {
                               accounttype=e.accountType,
                               accountname = e.name,
                               debit = vb.debit,
                               credit = vb.credit
                           }).ToList();

            var preveous = (from e in db.ExpenseAccounts
                        where (e.accountType == "Revenue" || e.accountType == "Expense") && (e.isGroup == false) && (e.isActive == true)
                        join vb in db.VoucherBodies on e.code equals vb.accountNo
                        join v in db.Vouchers on vb.voucherNo equals v.voucherNo
                        where (v.voucherDate < startDate)
                        select new
                        {
                            accounttype = e.accountType,
                            accountname = e.name,
                            debit = vb.debit,
                            credit = vb.credit
                        }).ToList();
            var opening = db.ExpenseAccounts.Where(r => r.isGroup == false && r.isActive == true && (Math.Abs(r.openingDebit - r.openingCredit)) > 0);
            foreach (var j in opening)
            {
                data.Insert(0, new { accounttype = j.accountType, accountname = j.name + " Opening", debit = (j.openingDebit - j.openingCredit) >= 0 ? (j.openingDebit - j.openingCredit) : 0, credit = (j.openingDebit - j.openingCredit) < 0 ? (j.openingDebit - j.openingCredit) : 0 });
            }
            foreach (var i in preveous)
            {
                data.Insert(1, new { accounttype = i.accounttype, accountname = i.accountname + " Preveous", debit = i.debit, credit = i.credit });
            }
            var revenue = data.Where(r=>r.accounttype=="Revenue").GroupBy(i => new { i.accountname }).Select(s =>
            {
                return new {accountname = s.Select(c => c.accountname).Distinct(), balance = (s.Sum(c => c.debit) - s.Sum(c => c.credit)) };
            });
            var expense = data.Where(r => r.accounttype == "Expense").GroupBy(i => new { i.accountname }).Select(s =>
            {
                 return new { accountname = s.Select(c => c.accountname).Distinct(), balance = (s.Sum(c => c.debit) - s.Sum(c => c.credit)) };
            });
            var result = new { Revenue = revenue, Expense = expense };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult balanceSheet()
        {
            return View();
        }
        public JsonResult balSheet(DateTime startDate, DateTime endDate)
        {
            decimal revenueTot = 0;
            decimal expenseTot = 0;
            var data = (from e in db.ExpenseAccounts
                        where (e.accountType == "Assets" || e.accountType == "Liability" || e.accountType == "Capital" || e.accountType == "Revenue" || e.accountType == "Expense") && (e.isGroup == false) && (e.isActive==true)
                       join vb in db.VoucherBodies on e.code equals vb.accountNo
                       join v in db.Vouchers on vb.voucherNo equals v.voucherNo where (v.voucherDate>=startDate) && (v.voucherDate<=endDate)
                       select new
                       {
                           accounttype=e.accountType,
                           accountname=e.name,
                           debit=vb.debit,
                           credit=vb.credit
                       }).ToList();
            var opening = db.ExpenseAccounts.Where(r => r.isActive == true && r.isGroup == false && (Math.Abs(r.openingDebit - r.openingCredit)) > 0);
            var preveous = (from e in db.ExpenseAccounts
                        where (e.accountType == "Assets" || e.accountType == "Liability" || e.accountType == "Capital" || e.accountType == "Revenue" || e.accountType == "Expense") && (e.isGroup == false) && (e.isActive == true)
                        join vb in db.VoucherBodies on e.code equals vb.accountNo
                        join v in db.Vouchers on vb.voucherNo equals v.voucherNo
                        where (v.voucherDate < startDate)
                        select new
                        {
                            accounttype = e.accountType,
                            accountname = e.name,
                            debit = vb.debit,
                            credit = vb.credit
                        }).ToList();
            foreach (var i in opening)
            {
                data.Insert(0, new { accounttype = i.accountType, accountname = i.name + " Opening", debit = (i.openingDebit - i.openingCredit) >= 0 ? (i.openingDebit - i.openingCredit) : 0, credit = (i.openingDebit - i.openingCredit) < 0 ? (i.openingDebit - i.openingCredit) : 0 });
            }
            foreach (var j in preveous)
            {
                data.Insert(1, new { accounttype = j.accounttype, accountname = j.accountname + " Preveous", debit = j.debit, credit = j.credit });
            }
            var assets = data.Where(r => r.accounttype == "Assets").GroupBy(i => new { i.accountname }).Select(s =>
            {
                return new { accountname = s.Select(c => c.accountname).Distinct(), balance = (s.Sum(c => c.debit) - s.Sum(c => c.credit)) };
            });
            var liability = data.Where(r => r.accounttype == "Liability").GroupBy(i => new { i.accountname }).Select(s =>
            {
                return new { accountname = s.Select(c => c.accountname).Distinct(), balance = (s.Sum(c => c.debit) - s.Sum(c => c.credit)) };
            });
            var capital = data.Where(r => r.accounttype == "Capital").GroupBy(i => new { i.accountname }).Select(s =>
            {
                return new { accountname = s.Select(c => c.accountname).Distinct(), balance = (s.Sum(c => c.debit) - s.Sum(c => c.credit)) };
            });
            var revenue = data.Where(r => r.accounttype == "Revenue").GroupBy(i => new { i.accountname }).Select(s =>
            {
                revenueTot += (s.Sum(c => c.debit) - s.Sum(c => c.credit));
                return new { revenueTot };
            });
            var expense = data.Where(r => r.accounttype == "Expense").GroupBy(i => new { i.accountname }).Select(s =>
            {
                expenseTot += (s.Sum(c => c.debit) - s.Sum(c => c.credit));
                return new { expenseTot };
            });
            var result = new { Assets = assets, Liability = liability, Capital = capital, Revenue=revenue, Expense=expense };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult saleReport()
        {
            return View();
        }
        public ActionResult purchaseReport()
        {
            return View();
        }
        public JsonResult PurchaseR(DateTime startDate, DateTime endDate)
        {
            var data = (from p in db.Purchases
                        join pd in db.PurchaseDetails on p.purchaseId equals pd.purchaseDetailsId
                        where (p.purchaseDate >= startDate) && (p.purchaseDate <= endDate)
                        select new
                        {
                            purchaseId = p.purchaseId,
                            purchasedate = p.purchaseDate,
                            vendorname=p.vendorName,
                            //productname=pd.productName,
                            qty = pd.quantity, 
                            //price = pd.purchasePrice,
                            amount = pd.amount
                        }).ToList();
            var result = data.GroupBy(r => r.purchaseId).Select(s =>
            {
                return new { amount = s.Sum(f=>f.amount), purchaseId=s.Select(f=>f.purchaseId).Distinct(), purchasedate=s.Select(f=>f.purchasedate).Distinct(),
                    vendorname=s.Select(f=>f.vendorname).Distinct(), qty=s.Sum(f=>f.qty)
                };
            });
            var value = new { Data = data, ptot = result };
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaleR(DateTime startDate, DateTime endDate)
        {
            var data = (from s in db.Sales
                        join sd in db.SaleDetails on s.saleId equals sd.saleDetailsId
                        where (s.saleDate >= startDate) && (s.saleDate <= endDate)
                        select new
                        {
                            saleId = s.saleId,
                            saledate = s.saleDate,
                            customername = s.customerName,
                            qty = sd.quantity,
                            amount = sd.amount
                        }).ToList();
            var result = data.GroupBy(r => r.saleId).Select(s =>
            {
                return new
                {
                    amount = s.Sum(f => f.amount),
                    saleId = s.Select(f => f.saleId).Distinct(),
                    saledate = s.Select(f => f.saledate).Distinct(),
                    customername = s.Select(f => f.customername).Distinct(),
                    qty = s.Sum(f => f.qty)
                };
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}