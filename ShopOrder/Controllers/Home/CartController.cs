using ShopOrder.Controllers.Model;
using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ShopOrder.Controllers.Home
{
    public class CartController : BaseController
    {
        ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            DKHACHHANG userLogin = CookieUtils.UserLogin().dKHACHHANG;
            List<TDONHANGCHITIET> gioHangs = LayGioHang(userLogin);
            if (gioHangs != null && gioHangs.Count > 0)
            {
                var tmp = db.TDONHANGCHITIETs.Where(x => x.DKHACHHANGID == userLogin.ID && x.SOLUONG <= 0).ToList();
                if (tmp != null && tmp.Count > 0)
                {
                    db.TDONHANGCHITIETs.RemoveRange(tmp);
                    db.SaveChanges();
                }
            }

            return View(gioHangs);
        }

        [HttpGet]
        public ActionResult ThongTinDatHang(string id)
        {
            if (id == null || id.Length == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                UserModel userLogin = CookieUtils.UserLogin();
                ViewBag.View = true;
                ViewBag.Title = "Chi tiết đơn hàng";
                TDONHANG dhRow = db.TDONHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (dhRow == null || dhRow.DATHANHTOAN == 30) return HttpNotFound();
                ViewBag.LOAIVANCHUYEN = KhachHangController.LayLoaiVanChuyen(false, dhRow.LOAIVANCHUYEN ?? 0);
                ViewBag.DNHAXEID = new SelectList(db.DNHAXEs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dhRow.DNHAXEID);
                ViewBag.Layout = UiUtils.Layout(userLogin);
                ViewBag.IsCustomer = CookieUtils.UserLogin().IsCustomer;
                return View(dhRow);
            }
        }

        [HttpPost]
        public ActionResult ThongTinDatHang()
        {
            string[] data = Request.Params.GetValues("ids[]");
            if (data == null || data.Length == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                ViewBag.View = false;
                ViewBag.Title = "Giỏ hàng";
                List<TDONHANGCHITIET> lst = db.TDONHANGCHITIETs.Where(x => data.Contains(x.ID)).ToList();
                TDONHANG dhRow = new TDONHANG();
                dhRow.TDONHANGCHITIETs = lst;
                dhRow.DKHACHHANG = CookieUtils.UserLogin().dKHACHHANG;
                dhRow.TIENHANG = lst.Sum(x => x.THANHTIEN);
                dhRow.PHIVANCHUYEN = 0;
                dhRow.TONGCONG = dhRow.TIENHANG + dhRow.PHIVANCHUYEN;
                ViewBag.LOAIVANCHUYEN = KhachHangController.LayLoaiVanChuyen(false, dhRow.DKHACHHANG.LOAIVANCHUYEN ?? 0);
                ViewBag.DNHAXEID = new SelectList(db.DNHAXEs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dhRow.DKHACHHANG.DNHAXEID);
                ViewBag.IsCustomer = CookieUtils.UserLogin().IsCustomer;
                ViewBag.Layout = UiUtils.Layout();
                return View(dhRow);
            }
        }

        [HttpGet]
        public ActionResult HuyDatHang(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Find(id);
            if (dhRow == null || dhRow.DATHANHTOAN == 30) return HttpNotFound();
            //Xóa lưu vết
            db.TLUUVETs.RemoveRange(db.TLUUVETs.Where(x => x.TDONHANGID == dhRow.ID).ToList());
            db.SaveChanges();

            //Xóa chi tiết
            db.TDONHANGCHITIETs.RemoveRange(db.TDONHANGCHITIETs.Where(x => x.TDONHANGID == dhRow.ID).ToList());
            db.SaveChanges();

            //Xoá đơn hàng
            db.TDONHANGs.RemoveRange(db.TDONHANGs.Where(x => x.ID == dhRow.ID).ToList());
            db.SaveChanges();

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public ActionResult ThongTinThanhToan(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Where(x => x.DATHANHTOAN != 30 && x.ID == id).FirstOrDefault();
            if (dhRow == null || dhRow.DATHANHTOAN == 30) return HttpNotFound();
            dhRow.TMPCODE = UiUtils.ImageQrCode(dhRow);
            return View(dhRow);
        }

        [HttpPost]
        public ActionResult ThongTinThanhToan(TDONHANG tmp)
        {
            string[] data = Request.Params.GetValues("ids[]");
            if (data == null || data.Length == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                TDONHANG dhRow;
                if (tmp.ID == null)
                {
                    dhRow = new TDONHANG();
                    List<TDONHANGCHITIET> lst = db.TDONHANGCHITIETs.Where(x => data.Contains(x.ID)).ToList();
                    dhRow.ID = Guid.NewGuid().ToString();
                    dhRow.LOAI = 0;
                    dhRow.TIMECREATED = DateTime.Now;
                    dhRow.NGAY = DateTime.Now.Date;
                    dhRow.NAME = DatabaseUtils.GenCode("NAME", "TDONHANG");
                    dhRow.TINHTHANH = tmp.TINHTHANH;
                    dhRow.QUANHUYEN = tmp.QUANHUYEN;
                    dhRow.PHUONGXA = tmp.PHUONGXA;
                    dhRow.DIACHI = tmp.DIACHI;
                    dhRow.DATHANHTOAN = 0;
                    dhRow.LOAIVANCHUYEN = tmp.LOAIVANCHUYEN;
                    if ((dhRow.LOAIVANCHUYEN ?? 0) == (int)LoaiVanChuyen.GuiXe)
                    {
                        dhRow.DNHAXEID = tmp.DNHAXEID;
                    }
                    dhRow.DKHACHHANGID = lst[0].DKHACHHANGID;
                    dhRow.TIENHANG = lst.Sum(x => x.THANHTIEN);
                    dhRow.PHIVANCHUYEN = 0;
                    dhRow.TONGCONG = dhRow.TIENHANG + dhRow.PHIVANCHUYEN;
                    dhRow.TMPCODE = LayCodeChuyenKhoan(dhRow.DKHACHHANGID);
                    db.TDONHANGs.Add(dhRow);
                    db.SaveChanges();

                    //log
                    DatabaseUtils.Log(dhRow, "Tạo đơn");

                    foreach (var ctRow in lst)
                    {
                        ctRow.TDONHANGID = dhRow.ID;
                        db.Entry(ctRow);
                    }
                    db.SaveChanges();
                }
                else
                {
                    dhRow = db.TDONHANGs.Find(tmp.ID);
                }
                dhRow.TMPCODE = UiUtils.ImageQrCode(dhRow);
                return RedirectToAction("ThongTinThanhToan", "Cart", new { id = dhRow.ID });
            }
        }

        private string LayCodeChuyenKhoan(string DKHACHHANGID)
        {
            DKHACHHANG dKHACHHANG = db.DKHACHHANGs.Find(DKHACHHANGID);
            string code = PasswordUtils.RemoveUnicodeAndSpace(dKHACHHANG.USERNAME.Trim());
            code += DateTime.Now.ToString("yyMMddHHmmss");
            return code;
        }

        private List<TDONHANGCHITIET> LayGioHang(DKHACHHANG userLogin)
        {
            return db.TDONHANGCHITIETs.Where(x => x.TDONHANG == null && x.DKHACHHANGID == userLogin.ID).ToList();
        }

        public ActionResult AddToCart(TDONHANGCHITIET item)
        {
            DKHACHHANG userLogin = CookieUtils.UserLogin().dKHACHHANG;
            string error = "";
            if (item == null || item.DMATHANGID == null || item.DMATHANGID.Length == 0)
            {
                error = "Có lỗi trong quá trình đặt hàng";
            }
            else if (item.DSIZEID == null)
            {
                error = "Size chưa được chọn";
            }
            else if (item.DMAUID == null)
            {
                error = "Màu chưa được chọn";
            }
            else if (item.SOLUONG <= 0)
            {
                error = "Số lượng không hợp lệ";
            }
            else
            {
                List<TDONHANGCHITIET> gioHangs = LayGioHang(userLogin);
                //kiểm tra xem chi tiết có chưa, có thì cộng số lượng, chưa có thì thêm
                bool added = false;
                if (gioHangs != null && gioHangs.Count > 0)
                {
                    foreach (TDONHANGCHITIET ctRow in gioHangs)
                    {
                        if (item.DMATHANGID == ctRow.DMATHANGID && item.DONGIA == ctRow.DONGIA
                            && item.DSIZEID == ctRow.DSIZEID && item.DMAUID == ctRow.DMAUID)
                        {
                            ctRow.SOLUONG += item.SOLUONG;
                            ctRow.THANHTIEN = ctRow.SOLUONG * ctRow.DONGIA;
                            db.Entry(ctRow);
                            added = true;
                        }
                    }
                }

                if (!added)
                {
                    TDONHANGCHITIET ctRow = new TDONHANGCHITIET();
                    ctRow.ID = Guid.NewGuid().ToString();
                    ctRow.DKHACHHANGID = userLogin.ID;
                    ctRow.DONGIA = item.DONGIA;
                    ctRow.SOLUONG = item.SOLUONG;
                    ctRow.THANHTIEN = ctRow.SOLUONG * ctRow.DONGIA;
                    ctRow.DMATHANGID = item.DMATHANGID;
                    ctRow.DMAUID = item.DMAUID;
                    ctRow.DSIZEID = item.DSIZEID;
                    db.TDONHANGCHITIETs.Add(ctRow);
                }
                db.SaveChanges();
            }
            return Content(error.Length == 0 ? "ok" : error);
        }

        public ActionResult ThayDoiSoLuong(TDONHANGCHITIET item)
        {
            TDONHANGCHITIET ctRow = db.TDONHANGCHITIETs.Find(item.ID);
            ctRow.SOLUONG = item.SOLUONG;
            ctRow.THANHTIEN = ctRow.DONGIA * ctRow.SOLUONG;
            db.Entry(ctRow);
            db.SaveChanges();
            return Content("ok");
        }
    }
}