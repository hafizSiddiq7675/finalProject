using System.Linq;
using System.Net;
using System.Web.Mvc;
using businessProBms.Models;
using PagedList;
namespace businessProBms.Controllers
{
    public class ProductController : Controller
    {
        private businessProBmsEntities db = new businessProBmsEntities();

        // GET: /Product/
        public ActionResult Add(int? page)
        {
            Product p = new Product();
            var maxId = db.Products.ToList().OrderByDescending(r => r.productCode).FirstOrDefault();
            p.productCode = maxId == null ? 1 : (maxId.productCode) + 1;
            ViewBag.Products = (db.Products.ToList().ToPagedList(page ?? 1,8));
            ViewBag.cat = db.Categories.ToList();
            //ViewBag.cat = new SelectList(db.Categories.ToList(), "categoryName", "categoryName");
            return View(p);
        }
        //POST:Product/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include="productCode,categoryCode,categoryName,name,UOM,price")] Product product, int? page)
        {
            if(ModelState.IsValid)
            {
                var pr = db.Products.SingleOrDefault(m => m.productCode == product.productCode);
                ExpenseAccount exSale = new ExpenseAccount();
                ExpenseAccount exPurchase = new ExpenseAccount();
                if (pr == null)
                {
                    var max = db.ExpenseAccounts.ToList().OrderByDescending(r => r.code).FirstOrDefault();
                    exPurchase.code = max == null ? 1 : (max.code) + 1;
                    exPurchase.accountType = "Expense";
                    exPurchase.description = "Purchase Account";
                    exPurchase.isGroup = false;
                    exPurchase.name = product.name + " Purchase";
                    exPurchase.openingCredit = 0;
                    exPurchase.openingDebit = 0;
                    exPurchase.parentCode = 3;
                    db.ExpenseAccounts.Add(exPurchase);
                    db.SaveChanges();
                    var maxCode = db.ExpenseAccounts.ToList().OrderByDescending(r => r.code).FirstOrDefault();
                    exSale.code = maxCode == null ? 1 : (maxCode.code) + 1;
                    exSale.accountType = "Revenue";
                    exSale.description = "Sale Account";
                    exSale.isGroup = false;
                    exSale.name = product.name + " Sale";
                    exSale.openingCredit = 0;
                    exSale.openingDebit = 0;
                    exSale.parentCode = 2;
                    db.ExpenseAccounts.Add(exSale);
                    db.SaveChanges();
                    product.chartOfAccCode = exPurchase.code;
                    db.Products.Add(product);
                }
                else
                {
                    var prDb = db.Products.Single(m => m.productCode == pr.productCode);
                    prDb.productCode = product.productCode;
                    prDb.name = product.name;
                    prDb.UOM = product.UOM;
                    prDb.categoryCode = product.categoryCode;
                    prDb.categoryName = product.categoryName;
                    prDb.price = product.price;
                }
                db.SaveChanges();
                return RedirectToAction("Add");
            }
            ViewBag.cat = db.Categories.ToList();
            ViewBag.Products = (db.Products.ToList().ToPagedList(page ?? 1, 8));
            //ViewBag.Products = (db.Products.ToList().ToPagedList(page ?? 1, 8));
            //ViewBag.cat = new SelectList(db.Categories.ToList(), "categoryName", "categoryName");
            return View(product);
        }
        public ActionResult Edit(int? id, int? page)
        {
            ViewBag.Products = db.Products.ToList().ToPagedList(page?? 1,8);
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pr = db.Products.SingleOrDefault(m => m.productCode == id);
            
            if(pr==null)
            {
                return HttpNotFound();
            }
            ViewBag.cat = db.Categories.ToList();
            return View("Add", pr);
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Product pr = db.Products.SingleOrDefault(m => m.productCode == id);
            if(pr!=null)
            {
                db.Products.Remove(pr);
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
