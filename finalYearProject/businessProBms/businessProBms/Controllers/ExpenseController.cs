using System.Linq;
using System.Web.Mvc;
using businessProBms.Models;
using System.Net;
using PagedList;
using PagedList.Mvc;
using System;
namespace businessProBms.Controllers
{
    public class ExpenseController : Controller
    {
        businessProBmsEntities db = new businessProBmsEntities();
        // GET: /Expense/Expenses
        public ActionResult Expenses(int? page)
        {
            ExpenseAccount ex = new ExpenseAccount();
            var maxId = db.ExpenseAccounts.ToList().OrderByDescending(r => r.code).FirstOrDefault();
            ex.code = maxId == null ? 1 : (maxId.code) + 1;
            ViewBag.parentAcc = db.ExpenseAccounts.Where(r => r.isGroup == true).ToList();
            ViewBag.expenses = (db.ExpenseAccounts.Where(r=>r.isActive==true).ToList().ToPagedList(page ?? 1, 8));
            return View(ex);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Expenses([Bind(Include="accountType,code,name,description,parentCode,isGroup,openingDebit,openingCredit, isActive")] ExpenseAccount exp)
        {
            if(ModelState.IsValid)
            {
                ExpenseAccount e = db.ExpenseAccounts.Find(exp.code);
                if (e == null)
                {
                    exp.isActive = true;
                    db.ExpenseAccounts.Add(exp);
                }
                else
                {
                    var exDb = db.ExpenseAccounts.Single(x => x.code == exp.code);
                    exDb.accountType = exp.accountType;
                    exDb.code = exp.code;
                    exDb.name = exp.name;
                    exDb.parentCode = exp.parentCode;
                    exDb.isGroup = exp.isGroup;
                    exDb.openingDebit = exp.openingDebit;
                    exDb.openingCredit = exp.openingCredit;
                }
                db.SaveChanges();
                return RedirectToAction("Expenses");
            }
            return View(exp);
        }
        public ActionResult Edit(int? id, int? page)
        {
            ViewBag.expenses = (db.ExpenseAccounts.ToList().ToPagedList(page ?? 1, 8));
            ViewBag.parentAcc = db.ExpenseAccounts.Where(r => r.isGroup == true).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpenseAccount exp = db.ExpenseAccounts.Find(id);
            if(exp==null)
            {
                return HttpNotFound();
            }
            return View("Expenses", exp);
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            ExpenseAccount exp = db.ExpenseAccounts.SingleOrDefault(m => m.code == id);
            if(exp!=null)
            {
                db.ExpenseAccounts.Remove(exp);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}