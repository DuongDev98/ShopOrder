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
using ShopOrder.Utils;

namespace ShopOrder.Controllers.Model
{
    
    public class NhaXeController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View(db.DNHAXEs.OrderBy(x => x.NAME).ToList());
        }

        public ActionResult Edit(string ID)
        {
            DNHAXE model;
            if (ID == null)
            {
                model = new DNHAXE();
            }
            else
            {
                model = db.DNHAXEs.Find(ID);
            }
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DNHAXE model)
        {
            if (ModelState.IsValid)
            {
                if (model.ID == null)
                {
                    model.ID = Guid.NewGuid().ToString();
                    resetValue(model);
                    db.DNHAXEs.Add(model);
                }
                else
                {
                    resetValue(model);
                    db.Entry(model).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        private void resetValue(DNHAXE model)
        {
            if ((model.XEOM ?? 0) > 0)
            {
                model.BIENSO = null;
                model.BENDO = null;
                model.GIOXECHAY = null;
            }
        }

        public ActionResult Delete(string ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DNHAXE model = db.DNHAXEs.Find(ID);
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
            DNHAXE model = db.DNHAXEs.Find(ID);
            db.DNHAXEs.Remove(model);
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
