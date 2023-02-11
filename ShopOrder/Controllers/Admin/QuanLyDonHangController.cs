﻿using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers.Home
{
    public class QuanLyDonHangController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();
        public ActionResult Index()
        {
            UserModel userLogin = CookieUtils.UserLogin();
            Dictionary<DTRANGTHAIDON, int> dic = new Dictionary<DTRANGTHAIDON, int>();
            IQueryable<TDONHANG> donHangs = db.TDONHANGs.Where(x=>x.LOAI == 0);
            //khách hàng chỉ hiện thị đơn của nó
            if (userLogin.IsCustomer) donHangs = donHangs.Where(x => x.DKHACHHANGID == userLogin.dKHACHHANG.ID);
            dic.Add(new DTRANGTHAIDON() { ID = "", NAME = "Chờ thanh toán" }, donHangs.Count(x=>x.DATHANHTOAN != 30));

            IQueryable<TDONHANGCHITIET> donHangChiTiets = db.TDONHANGCHITIETs;
            if (userLogin.IsCustomer) donHangChiTiets = donHangChiTiets.Where(x => x.DKHACHHANGID == userLogin.dKHACHHANG.ID);

            var tmp = db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList();
            foreach (var item in tmp)
            {
                dic.Add(item, donHangChiTiets.Count(x => x.TDONHANG.DATHANHTOAN == 30 && x.DTRANGTHAIDONID == item.ID));
            }

            ViewBag.Layout = UiUtils.Layout(userLogin);
            return View(dic);
        }

        public ActionResult DanhSach(string id)
        {
            if (id != null && id.Length > 0)
            {
                
            }
            return RedirectToAction("DanhSachChuaThanhToan", "QuanLyDonHang");
        }

        public ActionResult DanhSachChiTiet(string id)
        {
            string title = "Chi tiết";
            UserModel userLogin = CookieUtils.UserLogin();
            IQueryable<TDONHANGCHITIET> result = db.TDONHANGCHITIETs.Where(x => x.DTRANGTHAIDONID == id);
            if (userLogin.IsCustomer)
            {
                result = result.Where(x => x.DKHACHHANGID == userLogin.dKHACHHANG.ID);
            }
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.Title = title;
            ViewBag.Layout = UiUtils.Layout(userLogin);
            return View(result.ToList());
        }

        public ActionResult DanhSachChuaThanhToan()
        {
            UserModel userLogin = CookieUtils.UserLogin();
            IQueryable<TDONHANG> result = db.TDONHANGs.Where(x => x.LOAI == 0 && x.DATHANHTOAN != 30);
            if (userLogin.IsCustomer)
            {
                result = result.Where(x=>x.DKHACHHANGID == userLogin.dKHACHHANG.ID);
            }
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.Title = "Danh sách chưa thanh toán";
            ViewBag.Layout = UiUtils.Layout(userLogin);
            return View(result.ToList());
        }

        public ActionResult DoiTrangThai(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Find(id);
            if (dhRow == null) return HttpNotFound();
            if (dhRow.DATHANHTOAN != 30)
            {
                dhRow.TIENTHANHTOAN = dhRow.TONGCONG;
                dhRow.DATHANHTOAN = 30;
                db.Entry(dhRow);
                db.SaveChanges();

                //log
                DatabaseUtils.Log(dhRow, "Đã thanh toán");
            }
            return RedirectToAction("DanhSach", "QuanLyDonHang");
        }

        [HttpPost]
        public ActionResult KiemTraThanhToan(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Where(x => x.ID == id).FirstOrDefault();
            if (dhRow != null && dhRow.DATHANHTOAN == 30)
            {
                return Content("ok");
            }
            //thực hiện truy vấn kiểm tra cú pháp tin nhắn có tồn tại không
            return Content("error");
        }

        public ActionResult CapTrangThaiThanhToan()
        {
            List<TDONHANG> lst = db.TDONHANGs.Where(x => x.DATHANHTOAN != 30).ToList();
            foreach (TDONHANG item in lst)
            {
                //thực hiện truy vấn kiểm tra cú pháp tin nhắn có tồn tại không
                bool kq = false;
                if (kq)
                {
                    item.DATHANHTOAN = 30;
                    db.Entry(item);
                    db.SaveChanges();

                    //log
                    DatabaseUtils.Log(item, "Đã thanh toán");
                }
            }
            return RedirectToAction("DanhSach", "QuanLyDonHang");
        }
    }
}