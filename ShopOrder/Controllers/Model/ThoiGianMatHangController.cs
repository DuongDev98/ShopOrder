using ShopOrder.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers.Model
{
    public class ThoiGianMatHangController : Controller
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View(db.DTHOIGIANDATs.OrderBy(x => x.NAME).ToList());
        }

        public ActionResult Edit(string ID)
        {
            DTHOIGIANDAT model;
            if (ID == null)
            {
                model = new DTHOIGIANDAT();
            }
            else
            {
                model = db.DTHOIGIANDATs.Find(ID);
            }
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DTHOIGIANDAT model)
        {
            if (ModelState.IsValid)
            {
                if (model.ID == null)
                {
                    model.ID = Guid.NewGuid().ToString();
                    db.DTHOIGIANDATs.Add(model);
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
            DTHOIGIANDAT model = db.DTHOIGIANDATs.Find(ID);
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
            DTHOIGIANDAT model = db.DTHOIGIANDATs.Find(ID);
            db.DTHOIGIANDATs.Remove(model);
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