﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopOrder.Commons;
using ShopOrder.Controllers.Model;
using ShopOrder.Entities;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ShopOrder.Controllers.Admin
{
    public class QuanLyDonGiaoController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index(ParamInfo info)
        {
            if (info == null)
            {
                info = new ParamInfo();
                info.LOAIVANCHUYEN = -1;
            }

            //param
            string[] splitDate = info.dtNGAY.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            DateTime fromDate = DateTime.Parse(splitDate[0]);
            DateTime toDate = DateTime.Parse(splitDate[1]);

            //filter
            IQueryable<TGIAOHANG> lst = db.TGIAOHANGs;
            lst = lst.Where(x => x.NGAY >= fromDate && x.NGAY <= toDate);
            lst = lst.Where(x => x.DKHACHHANGID == info.DKHACHHANGID || info.DKHACHHANGID == null || info.DKHACHHANGID == "");
            lst = lst.Where(x => x.DNHANVIENID == info.DNHANVIENID || info.DNHANVIENID == null || info.DNHANVIENID == "");
            lst = lst.Where(x => x.LOAIVANCHUYEN == info.LOAIVANCHUYEN || info.LOAIVANCHUYEN == -1);
            lst = lst.Where(x => x.DNHAXEID == info.DNHAXEID || info.DNHAXEID == null || info.DNHAXEID == "");

            ViewBag.dtNGAY = info.dtNGAY;

            List<DKHACHHANG> lstKhachHang = db.DKHACHHANGs.OrderBy(x => x.NAME).ToList();
            lstKhachHang.Insert(0, new DKHACHHANG() { ID = "", NAME = "" });
            ViewBag.DKHACHHANGID = new SelectList(lstKhachHang, "ID", "NAME", info.DKHACHHANGID);

            List<DNHANVIEN> lstNhanVien = db.DNHANVIENs.OrderBy(x => x.NAME).ToList();
            lstNhanVien.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(lstNhanVien, "ID", "NAME", info.DNHANVIENID);

            ViewBag.LOAIVANCHUYEN = KhachHangController.LayLoaiVanChuyen(true, info.LOAIVANCHUYEN);

            List<DNHAXE> lstNhaXe = db.DNHAXEs.OrderBy(x => x.NAME).ToList();
            lstNhaXe.Insert(0, new DNHAXE() { ID = "", NAME = "" });
            ViewBag.DNHAXEID = new SelectList(lstNhaXe, "ID", "NAME", info.DNHAXEID);
            return View(lst.ToList());
        }

        public ActionResult Edit(string id)
        {
            TGIAOHANG model;
            if (id == null)
            {
                model = new TGIAOHANG();
                model.NGAY = DateTime.Now.Date;
                model.NAME = "Tự động";
            }
            else
            {
                model = db.TGIAOHANGs.Find(id);
            }
            if (model == null)
            {
                return HttpNotFound();
            }

            List<DKHACHHANG> lstKhachHang = db.DKHACHHANGs.OrderBy(x => x.NAME).ToList();
            lstKhachHang.Insert(0, new DKHACHHANG() { ID = "", NAME = "" });
            ViewBag.DKHACHHANGID = new SelectList(lstKhachHang, "ID", "NAME", model.DKHACHHANGID);

            List<DNHANVIEN> lstNhanVien = db.DNHANVIENs.OrderBy(x => x.NAME).ToList();
            lstNhanVien.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(lstNhanVien, "ID", "NAME", model.DNHANVIENID);

            List<DNHAXE> lstDNHAXEID = db.DNHAXEs.OrderBy(x => x.NAME).ToList();
            lstDNHAXEID.Insert(0, new DNHAXE() { ID = "", NAME = "" });
            ViewBag.DNHAXEID = new SelectList(lstDNHAXEID, "ID", "NAME", model.DNHAXEID);

            ViewBag.LOAIVANCHUYEN = KhachHangController.LayLoaiVanChuyen(false, model.LOAIVANCHUYEN ?? 0);
            return View(model);
        }

        public ActionResult FormThemChiTiet(string dkhachhangid)
        {
            IQueryable<TDONHANGCHITIET> chiTiets = db.TDONHANGCHITIETs.Where(x => x.DKHACHHANGID == dkhachhangid 
            && x.TDONHANG.DATHANHTOAN == 30 && x.SLNHAN != 30); ;
            return PartialView(chiTiets.ToList());
        }

        [HttpPost]
        public ActionResult Edit(TGIAOHANG tmp)
        {
            TGIAOHANG dhRow = new TGIAOHANG();
            if (ModelState.IsValid)
            {
                if (tmp.ID == null)
                {
                    dhRow.ID = Guid.NewGuid().ToString();
                    dhRow.TIMECREATED = DateTime.Now;
                    dhRow.NAME = DatabaseUtils.GenCode("NAME", "TGIAOHANG", 1);
                    if (tmp.DNHANVIENID != null) dhRow.DNHANVIENID = tmp.DNHANVIENID;
                    dhRow.DKHACHHANGID = tmp.DKHACHHANGID;
                    db.TGIAOHANGs.Add(dhRow);
                    db.SaveChanges();
                }
                else
                {
                    dhRow.ID = tmp.ID;
                }
                dhRow = db.TGIAOHANGs.Find(dhRow.ID);
                if (dhRow.LOAIVANCHUYEN == (int)LoaiVanChuyen.GiaoHangTietKiem && dhRow.LABEL_GHTK != null && dhRow.LABEL_GHTK.Length > 0)
                {
                    ViewBag.errorMessage = "Đơn hàng đã up lên ghtk, không thể thay đổi";
                    goto end;
                }
                else
                {
                    if (tmp.DNHANVIENID != null) dhRow.DNHANVIENID = tmp.DNHANVIENID;
                    dhRow.DKHACHHANGID = tmp.DKHACHHANGID;
                    dhRow.NGAY = tmp.NGAY;
                    dhRow.NOTE = tmp.NOTE;
                    dhRow.LOAIVANCHUYEN = tmp.LOAIVANCHUYEN;
                    dhRow.TINHTHANH = tmp.TINHTHANH;
                    dhRow.QUANHUYEN = tmp.QUANHUYEN;
                    dhRow.PHUONGXA = tmp.PHUONGXA;
                    dhRow.DIACHI = tmp.DIACHI;
                    if (dhRow.LOAIVANCHUYEN == (int)LoaiVanChuyen.GuiXe)
                    {
                        dhRow.DNHAXEID = tmp.DNHAXEID;
                    }
                    db.Entry(dhRow);
                    db.SaveChanges();

                    if (dhRow.TDONHANGCHITIETs != null)
                    {
                        foreach (TDONHANGCHITIET tmpCt in dhRow.TDONHANGCHITIETs)
                        {
                            TDONHANGCHITIET updateRow = db.TDONHANGCHITIETs.Find(tmpCt.ID);
                            updateRow.TGIAOHANGID = dhRow.ID;
                            db.Entry(updateRow);
                        }
                        db.SaveChanges();
                    }

                    //lưu chi tiết
                    foreach (TDONHANGCHITIET tmpCt in tmp.TDONHANGCHITIETs)
                    {
                        TDONHANGCHITIET updateRow = db.TDONHANGCHITIETs.Find(tmpCt.ID);
                        updateRow.TGIAOHANGID = dhRow.ID;
                        db.Entry(updateRow);
                    }
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                dhRow = tmp;
            }
            end:
            List<DKHACHHANG> lstKhachHang = db.DKHACHHANGs.OrderBy(x => x.NAME).ToList();
            lstKhachHang.Insert(0, new DKHACHHANG() { ID = "", NAME = "" });
            ViewBag.DKHACHHANGID = new SelectList(lstKhachHang, "ID", "NAME", dhRow.DKHACHHANGID);

            List<DNHANVIEN> lstNhanVien = db.DNHANVIENs.OrderBy(x => x.NAME).ToList();
            lstNhanVien.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(lstNhanVien, "ID", "NAME", dhRow.DNHANVIENID);

            List<DNHAXE> lstDNHAXEID = db.DNHAXEs.OrderBy(x => x.NAME).ToList();
            lstDNHAXEID.Insert(0, new DNHAXE() { ID = "", NAME = "" });
            ViewBag.DNHAXEID = new SelectList(lstDNHAXEID, "ID", "NAME", dhRow.DNHAXEID);

            ViewBag.LOAIVANCHUYEN = KhachHangController.LayLoaiVanChuyen(false, dhRow.LOAIVANCHUYEN ?? 0);
            return View(dhRow);
        }

        [HttpPost]
        public ActionResult LayThongTinKhach(string id)
        {
            DKHACHHANG khRow = db.DKHACHHANGs.Find(id);
            if (khRow == null) return HttpNotFound();
            JObject item = new JObject();
            item["ID"] = khRow.ID;
            item["NAME"] = khRow.NAME;
            item["TINHTHANH"] = khRow.TINHTHANH;
            item["QUANHUYEN"] = khRow.QUANHUYEN;
            item["PHUONGXA"] = khRow.PHUONGXA;
            item["DIACHI"] = khRow.DIACHI;
            item["LOAIVANCHUYEN"] = khRow.LOAIVANCHUYEN;
            item["DNHAXEID"] = khRow.DNHAXEID;
            return Content(JsonConvert.SerializeObject(item));
        }

        [HttpPost]
        public ActionResult DetailTbody(string id)
        {
            if (id == null) return HttpNotFound();
            TGIAOHANG dhRow = db.TGIAOHANGs.Find(id);
            if (dhRow == null) return HttpNotFound();
            return PartialView(dhRow.TDONHANGCHITIETs.ToList());
        }

        public ActionResult TaiDonGiaoHangTk(string id)
        {
            TGIAOHANG giaoHangRow = db.TGIAOHANGs.Find(id);
            if (giaoHangRow == null) return Content("Có lỗi trong quá trình up đơn");

            //detail
            List<Product> products = new List<Product>();
            foreach (TDONHANGCHITIET ctRow in giaoHangRow.TDONHANGCHITIETs)
            {
                Product p = new Product();
                p.name = ctRow.DMATHANG.NAME;
                p.price = (int)ctRow.DONGIA;
                p.quantity = (int)ctRow.SOLUONG;
                decimal kl = (int)(ctRow.DMATHANG.KHOILUONG ?? 0);
                p.weight = kl == 0 ? (decimal)0.1 : kl;
                p.product_code = "";
                products.Add(p);
            }
            //order
            Order order = new Order();
            order.id = giaoHangRow.ID;
            order.pick_name = "Bùi Kim Phượng";
            order.pick_money = 0;
            order.pick_province = "Hà Nội";
            order.pick_district = "Gia Lâm";
            order.pick_ward = "Ninh Hiệp";
            order.pick_address = "Xóm";
            order.pick_tel = "0357192939";
            order.pick_email = "0357192939";
            //
            order.name = giaoHangRow.DKHACHHANG.NAME;
            order.address = giaoHangRow.DIACHI;
            order.province = giaoHangRow.TINHTHANH;
            order.district = giaoHangRow.QUANHUYEN;
            order.ward = giaoHangRow.PHUONGXA;
            order.hamlet = "Khác";
            order.tel = "";
            order.email = "";
            //
            order.return_name = order.pick_name;
            order.return_address = order.pick_address;
            order.return_province = order.pick_province;
            order.return_district = order.pick_district;
            order.return_ward = order.pick_ward;
            order.return_tel = order.pick_tel;
            order.return_email = order.pick_email;

            OrderAddReqquest orderRequest = new OrderAddReqquest();
            orderRequest.products = products;
            orderRequest.order = order;

            GiaoHangTietKiem tools = new GiaoHangTietKiem();
            ResponseGhtk responseGhtk = tools.orderAdd(orderRequest);
            if (responseGhtk == null)
            {
                return Content("error: Giao hàng tiết kiệm ko phản hồi");
            }
            else
            {
                if (responseGhtk.success)
                {
                    giaoHangRow.LABEL_GHTK = responseGhtk.order.label;
                    db.Entry(giaoHangRow);
                    db.SaveChanges();
                }
                else
                {
                    return Content(responseGhtk.message);
                }
            }
            return Content("ok");
        }
    }
}