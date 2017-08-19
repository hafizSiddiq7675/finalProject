using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using businessProBms.Models;
namespace businessProBms.Controllers
{
    public class InventoryController : Controller
    {
        businessProBmsEntities db = new businessProBmsEntities();
        // GET: /Inventory/Inventory
        public ActionResult Inventory()
        {
            ViewBag.categories = db.Categories.OrderBy(r=>r.categoryName).ToList();
            return View();
        }
        public JsonResult getProducts(int? id)
        {
            var data = (from c in db.Categories
                        where (c.code == id)
                        join p in db.Products on c.code equals p.categoryCode
                        select new
                        {
                            pname = p.name,
                            uom = p.UOM,
                            price=p.price,
                            average = p.averagePrice,
                            lastprice = p.lastPurchasePrice,
                            qty = p.currentQuantity,
                            categoryName = c.categoryName
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}