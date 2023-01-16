using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ShopOrder.Entities;
using ShopOrder.Utils;

namespace ShopOrder.Controllers.Model
{
    
    public class NhomKhachHangController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View(db.DNHOMKHACHHANGs.OrderBy(x => x.NAME).ToList());
        }

        public ActionResult Edit(string ID)
        {
            DNHOMKHACHHANG model;
            if (ID == null)
            {
                model = new DNHOMKHACHHANG();
            }
            else
            {
                model = db.DNHOMKHACHHANGs.Find(ID);
            }
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.LOAIGIA = GetLoaiGia(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DNHOMKHACHHANG model)
        {
            if (ModelState.IsValid)
            {
                if (model.ID == null)
                {
                    model.ID = Guid.NewGuid().ToString();
                    db.DNHOMKHACHHANGs.Add(model);
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LOAIGIA = GetLoaiGia(model);
            return View(model);
        }

        private SelectList GetLoaiGia(DNHOMKHACHHANG model)
        {
            List<object> lst = new List<object>();
            for (int i = 0; i < 4; i++)
            {
                lst.Add(new { ID = i, NAME = "Giá bán" + (i == 0 ? "" : " " + (i + 1).ToString()) });
            }
            return new SelectList(lst, "ID", "NAME", model.LOAIGIA);
        }

        public ActionResult Delete(string ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DNHOMKHACHHANG model = db.DNHOMKHACHHANGs.Find(ID);
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
            DNHOMKHACHHANG model = db.DNHOMKHACHHANGs.Find(ID);
            db.DNHOMKHACHHANGs.Remove(model);
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
