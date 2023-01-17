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
using ShopOrder.Models;
using ShopOrder.Utils;

namespace ShopOrder.Controllers.Model
{
    public class KhachHangController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            var dKHACHHANGs = db.DKHACHHANGs.Include(d => d.DNHAXE).Include(d => d.DNHOMKHACHHANG);
            return View(dKHACHHANGs.ToList());
        }

        public ActionResult Edit(string ID)
        {
            DKHACHHANG model;
            if (ID == null)
            {
                model = new DKHACHHANG();
            }
            else
            {
                model = db.DKHACHHANGs.Find(ID);
            }
            if (model == null)
            {
                return HttpNotFound();
            }

            UserModel userLogin = CookieUtils.UserLogin();
            ViewBag.Title = userLogin.IsCustomer ? "Thông tin cá nhân" : "Chỉnh sửa";
            ViewBag.Layout = "~/Views/Shared/" + (userLogin.IsCustomer ? "_Layout.cshtml" : "_Admin.cshtml");
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.DNHAXEID = new SelectList(db.DNHAXEs, "ID", "NAME", model.DNHAXEID);
            ViewBag.DNHOMKHACHHANGID = new SelectList(db.DNHOMKHACHHANGs, "ID", "NAME", model.DNHOMKHACHHANGID);
            ViewBag.LOAIVANCHUYEN = LayLoaiVanChuyen(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DKHACHHANG model)
        {
            if (ModelState.IsValid)
            {
                //Cập nhật dữ liệu
                DKHACHHANG khRow = db.DKHACHHANGs.Find(model.ID);
                model.USERNAME = khRow.USERNAME;
                model.PASSWORD = khRow.PASSWORD;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            UserModel userLogin = CookieUtils.UserLogin();
            ViewBag.Title = userLogin.IsCustomer ? "Thông tin cá nhân" : "Chỉnh sửa";
            ViewBag.Layout = "~/Views/Shared/" + (userLogin.IsCustomer ? "_Layout.cshtml" : "_Admin.cshtml");
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.DNHAXEID = new SelectList(db.DNHAXEs, "ID", "NAME", model.DNHAXEID);
            ViewBag.DNHOMKHACHHANGID = new SelectList(db.DNHOMKHACHHANGs, "ID", "NAME", model.DNHOMKHACHHANGID);
            ViewBag.LOAIVANCHUYEN = LayLoaiVanChuyen(model);
            return View(model);
        }

        private SelectList LayLoaiVanChuyen(DKHACHHANG khRow)
        {
            List<object> lst = new List<object>();
            lst.Add(new { ID = LoaiVanChuyen.GuiXe, NAME = "Gửi xe" });
            lst.Add(new { ID = LoaiVanChuyen.AnPhu, NAME = "Gửi bay An Phú" });
            lst.Add(new { ID = LoaiVanChuyen.GiaoHangTietKiem, NAME = "Giao hàng tiết kiệm" });
            return new SelectList(lst, "ID", "NAME", khRow.DNHAXEID);
        }

        public ActionResult Delete(string ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKHACHHANG model = db.DKHACHHANGs.Find(ID);
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
            DKHACHHANG model = db.DKHACHHANGs.Find(ID);
            db.DKHACHHANGs.Remove(model);
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

    public enum LoaiVanChuyen
    {
        GuiXe,
        AnPhu,
        GiaoHangTietKiem
    }
}
