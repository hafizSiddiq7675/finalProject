using System.Linq;
using System.Net;
using System.Web.Mvc;
using businessProBms.Models;
using PagedList;
namespace businessProBms.Controllers
{
    public class CategoryController : Controller
    {
        private businessProBmsEntities db = new businessProBmsEntities();

        // GET: /Category/Create
        public ActionResult Create(int? page)
        {
            //Giving all the list of Categories to ViewBag.Categories
            ViewBag.Categories = (db.Categories.ToList().ToPagedList(page ??1, 8));
            return View();
        }

        // POST: /Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="code,categoryName,discription")] Category category)
        {
            if (ModelState.IsValid)
            {
                Category cat = db.Categories.Find(category.code);
                if(cat==null)
                {
                    db.Categories.Add(category);
                }
                else
                {
                    var catDb = db.Categories.Single(m => m.code == category.code);
                    catDb.code = category.code;
                    catDb.categoryName = category.categoryName;
                    catDb.discription = category.discription;

                }
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(category);
        }
        // GET: /Category/Edit/5
        public ActionResult Edit(int? id, int? page)
        {
            ViewBag.Categories = db.Categories.ToList().ToPagedList(page ?? 1, 8);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View("Create", category);
        }

        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Category cat = db.Categories.SingleOrDefault(m => m.code == id);
            if(cat!=null)
            {
                db.Categories.Remove(cat);
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

