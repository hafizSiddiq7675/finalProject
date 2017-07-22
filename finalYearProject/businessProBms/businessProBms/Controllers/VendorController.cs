using System.Linq;
using System.Web.Mvc;
using businessProBms.Models;
using PagedList;
using System.Net;
namespace businessProBms.Controllers
{
    public class VendorController : Controller
    {
        private businessProBmsEntities db = new businessProBmsEntities();
        //
        // GET: /Vendor/Vendors
        public ActionResult Vendors(int? page)
        {
            ViewBag.vendors = (db.Vendors.ToList().ToPagedList(page?? 1,8));
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vendors([Bind(Include="code,name,contact,address,debitLimit")] Vendor vendor)
        {
            if(ModelState.IsValid)
            {
                Vendor v = db.Vendors.Find(vendor.code);
                if(v==null)
                {
                    db.Vendors.Add(vendor);
                }
                else
                {
                    var vDb = db.Vendors.SingleOrDefault(m => m.code == vendor.code);
                    vDb.code = vendor.code;
                    vDb.name = vendor.name;
                    vDb.contact = vendor.contact;
                    vDb.address = vendor.address;
                    vDb.debitLimit = vendor.debitLimit;
                }
                db.SaveChanges();
                return RedirectToAction("Vendors");
            }
            return View(vendor);
        }
        public ActionResult Edit(int? id, int?page)
        {
            ViewBag.vendors = (db.Vendors.ToList().ToPagedList(page ?? 1, 8));
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor v = db.Vendors.Find(id);
            if(v==null)
            {
                return HttpNotFound();
            }
            return View("Vendors", v);
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            Vendor v = db.Vendors.SingleOrDefault(m => m.code == id);
            if(v!=null)
            {
                db.Vendors.Remove(v);
                db.SaveChanges();
                result=true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}