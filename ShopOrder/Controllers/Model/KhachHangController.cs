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

        public ActionResult Edit(string id)
        {
            DKHACHHANG model;
            if (id == null)
            {
                model = new DKHACHHANG();
            }
            else
            {
                model = db.DKHACHHANGs.Find(id);
            }
            if (model == null)
            {
                return HttpNotFound();
            }
            UserModel userLogin = CookieUtils.UserLogin();
            ViewBag.Title = userLogin.IsCustomer ? "Thông tin cá nhân" : "Chỉnh sửa";
            ViewBag.Layout = UiUtils.Layout(userLogin);
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.DNHAXEID = new SelectList(db.DNHAXEs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", model.DNHAXEID);
            ViewBag.DNHOMKHACHHANGID = new SelectList(db.DNHOMKHACHHANGs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", model.DNHOMKHACHHANGID);
            ViewBag.LOAIVANCHUYEN = LayLoaiVanChuyen(false, model.LOAIVANCHUYEN ?? 0);
            //parentid
            ViewBag.parentId = new SelectList(db.DKHACHHANGs.Where(x=>x.ID != id).OrderBy(x => x.NAME).ToList(), "ID", "NAME", model.PARENTID);
            ViewBag.linkChiaSe = LayLinkChiaSe(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DKHACHHANG model)
        {
            DKHACHHANG khRow = null;
            if (ModelState.IsValid)
            {
                try
                {
                    //Cập nhật dữ liệu
                    khRow = db.DKHACHHANGs.Find(model.ID);
                    khRow.NAME = model.NAME;
                    khRow.DIENTHOAI = model.DIENTHOAI;
                    khRow.TINHTHANH = model.TINHTHANH;
                    khRow.QUANHUYEN = model.QUANHUYEN;
                    khRow.PHUONGXA = model.PHUONGXA;
                    khRow.DIACHI = model.DIACHI;
                    khRow.NOTE = model.NOTE;
                    khRow.LOAIVANCHUYEN = model.LOAIVANCHUYEN;
                    khRow.DNHAXEID = model.DNHAXEID;
                    khRow.DNHOMKHACHHANGID = model.DNHOMKHACHHANGID;
                    khRow.PARENTID = model.PARENTID;
                    db.Entry(khRow).State = EntityState.Modified;
                    db.SaveChanges();
                    if (!CookieUtils.UserLogin().IsCustomer)
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }
            UserModel userLogin = CookieUtils.UserLogin();
            ViewBag.Title = userLogin.IsCustomer ? "Thông tin cá nhân" : "Chỉnh sửa";
            ViewBag.Layout = UiUtils.Layout(userLogin);
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.DNHAXEID = new SelectList(db.DNHAXEs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", model.DNHAXEID);
            ViewBag.DNHOMKHACHHANGID = new SelectList(db.DNHOMKHACHHANGs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", model.DNHOMKHACHHANGID);
            ViewBag.LOAIVANCHUYEN = LayLoaiVanChuyen(false, model.LOAIVANCHUYEN ?? 0);
            //parentid
            ViewBag.parentId = new SelectList(db.DKHACHHANGs.Where(x => x.ID != model.ID).OrderBy(x => x.NAME).ToList(), "ID", "NAME", model.PARENTID);
            ViewBag.linkChiaSe = LayLinkChiaSe(model.ID);
            return View(khRow);
        }

        private string LayLinkChiaSe(string id)
        {
            if (id == null) return "";
            return Request.Url.Host + "/User/Register/" + id;
        }

        public static SelectList LayLoaiVanChuyen(bool filter, int loai)
        {
            List<object> lst = new List<object>();
            if (filter)
            {
                lst.Add(new { ID = -1, NAME = "" });
            }
            lst.Add(new { ID = (int)LoaiVanChuyen.GuiXe, NAME = "Gửi xe" });
            lst.Add(new { ID = (int)LoaiVanChuyen.AnPhu, NAME = "Gửi bay An Phú" });
            lst.Add(new { ID = (int)LoaiVanChuyen.GiaoHangTietKiem, NAME = "Giao hàng tiết kiệm" });
            return new SelectList(lst, "ID", "NAME", loai);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKHACHHANG model = db.DKHACHHANGs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DKHACHHANG model = db.DKHACHHANGs.Find(id);
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
        GuiXe = 0,
        AnPhu = 1,
        GiaoHangTietKiem = 2
    }
}
