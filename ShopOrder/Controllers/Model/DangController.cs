using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopOrder.Entities;

namespace ShopOrder.Controllers.Model
{
    public class DangController : Controller
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View(db.DDANGs.OrderBy(x=>x.NAME).ToList());
        }

        public ActionResult Edit(string ID)
        {
            DDANG model;
            if (ID == null)
            {
                model = new DDANG();
            }
            else
            {
                model = db.DDANGs.Find(ID);
            }
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DDANG model)
        {
            if (ModelState.IsValid)
            {
                if (model.ID == null)
                {
                    model.ID = Guid.NewGuid().ToString();
                    db.DDANGs.Add(model);
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Delete(string ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DDANG model = db.DDANGs.Find(ID);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string ID)
        {
            DDANG model = db.DDANGs.Find(ID);
            db.DDANGs.Remove(model);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string error = "";
                if (ex.GetType() == typeof(DbUpdateException) && ex.ToString().Contains("REFERENCE constraint"))
                {
                    error = "Dữ liệu đã phát sinh liên kết, không thể xóa";
                }
                else
                {
                    error = ex.ToString();
                }
                ViewBag.errorMessage = error;
                return View("Delete", model);
            }
            return RedirectToAction("Index");
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
