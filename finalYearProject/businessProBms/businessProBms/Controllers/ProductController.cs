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
            ViewBag.Products = (db.Products.ToList().ToPagedList(page ?? 1,8));
            ViewBag.cat = db.Categories.ToList();
            //ViewBag.cat = new SelectList(db.Categories.ToList(), "categoryName", "categoryName");
            return View();
        }
        //POST:Product/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include="code,categoryCode,categoryName,name,UOM,price")] Product product, int? page)
        {
            if(ModelState.IsValid)
            {
                var pr = db.Products.SingleOrDefault(m => m.code == product.code);
                if (pr == null)
                {
                    db.Products.Add(product);
                }
                else
                {
                    var prDb = db.Products.Single(m => m.code == pr.code);
                    prDb.code = product.code;
                    prDb.name = product.name;
                    prDb.UOM = product.UOM;
                    prDb.categoryCode = product.categoryCode;
                    prDb.categoryName = product.categoryName;
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
            var pr = db.Products.SingleOrDefault(m => m.code == id);
            
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
            Product pr = db.Products.SingleOrDefault(m => m.code == id);
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
