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
    
    public class NhanVienController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View(db.DNHANVIENs.OrderBy(x => x.NAME).ToList());
        }

        public ActionResult Edit(string id)
        {
            DNHANVIEN model;
            if (id == null)
            {
                model = new DNHANVIEN();
            }
            else
            {
                model = db.DNHANVIENs.Find(id);
            }
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.LOAITAIKHOAN = ListLoaiTaiKhoan(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DNHANVIEN model)
        {
            if (ModelState.IsValid)
            {
                if (model.ID == null)
                {
                    model.ID = Guid.NewGuid().ToString();
                    db.DNHANVIENs.Add(model);
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LOAITAIKHOAN = ListLoaiTaiKhoan(model);
            return View(model);
        }

        private SelectList ListLoaiTaiKhoan(DNHANVIEN model)
        {
            List<object> lst = new List<object>();
            object item = new { ID = (int)LoaiTaiKhoan.NhanVienSapHang, NAME = TextLoaiTaiKhoan(LoaiTaiKhoan.NhanVienSapHang) };
            lst.Add(item);
            item = new { ID = (int)LoaiTaiKhoan.NhanVienKho, NAME = TextLoaiTaiKhoan(LoaiTaiKhoan.NhanVienKho) };
            lst.Add(item);
            item = new { ID = (int)LoaiTaiKhoan.Shipper, NAME = TextLoaiTaiKhoan(LoaiTaiKhoan.Shipper) };
            lst.Add(item);
            item = new { ID = (int)LoaiTaiKhoan.CuaHang, NAME = TextLoaiTaiKhoan(LoaiTaiKhoan.CuaHang) };
            lst.Add(item);
            return new SelectList(lst, "ID", "NAME", model.LOAITAIKHOAN);
        }

        public static string TextLoaiTaiKhoan(LoaiTaiKhoan loai)
        {
            string kq = "";
            if (loai == LoaiTaiKhoan.NhanVienSapHang) kq = "Nhân viên sắp hàng";
            else if (loai == LoaiTaiKhoan.NhanVienKho) kq = "Nhân viên kho";
            else if (loai == LoaiTaiKhoan.Shipper) kq = "Shipper";
            else if (loai == LoaiTaiKhoan.CuaHang) kq = "Cửa hàng";
            return kq;
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DNHANVIEN model = db.DNHANVIENs.Find(id);
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
            DNHANVIEN model = db.DNHANVIENs.Find(id);
            db.DNHANVIENs.Remove(model);
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

    public enum LoaiTaiKhoan
    {
        NhanVienSapHang = 0,
        NhanVienKho = 1,
        Shipper = 2,
        CuaHang = 3
    }
}
