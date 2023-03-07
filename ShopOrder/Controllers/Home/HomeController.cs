using ShopOrder.Commons;
using ShopOrder.Controllers.Home;
using ShopOrder.Controllers.Model;
using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    public class HomeController : BaseController
    {
        int pageSize = 20;
        ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Header()
        {
            UserModel userModel = CookieUtils.UserLogin();
            ViewBag.ID = userModel.dKHACHHANG.ID;
            return PartialView();
        }

        //Danh sách mặt hàng
        public ActionResult DanhSachMatHang(ParamMatHangInfo info)
        {
            ViewBag.Title = "Danh sách mặt hàng";

            //tham số lọc
            ViewBag.loaiMatHang = info.loaiMatHang;
            ViewBag.page = info.page;
            ViewBag.dangId = info.dangId;
            ViewBag.mauId = info.mauId;
            ViewBag.nhomId = info.nhomId;
            ViewBag.phanLoaiId = info.phanLoaiId;
            ViewBag.shopId = info.shopId;
            ViewBag.sizeId = info.sizeId;
            ViewBag.thoiGianId = info.thoiGianId;

            //Danh sách dữ liệu
            ViewBag.dsDang = db.DDANGs.OrderBy(x=>x.NAME).ToList();
            ViewBag.dsMau = db.DMAUs.OrderBy(x => x.NAME).ToList();
            ViewBag.dsNhom = db.DNHOMHANGs.OrderBy(x => x.NAME).ToList();
            ViewBag.dsPhanLoai = db.DPHANLOAIs.OrderBy(x => x.NAME).ToList();
            ViewBag.dsShop = db.DNHANVIENs.Where(x=>x.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang).OrderBy(x => x.NAME).ToList();
            ViewBag.dsSize = db.DSIZEs.OrderBy(x => x.NAME).ToList();
            ViewBag.dsThoiGian = db.DTHOIGIANDATs.OrderBy(x => x.NAME).ToList();
            return View();
        }

        public ActionResult ListItemMatHang(ParamMatHangInfo info)
        {
            IQueryable<DMATHANG> lstData = db.DMATHANGs;
            if (info.loaiMatHang >= 0)
            {
                if (info.loaiMatHang == (int)LoaiHang.HangSale)
                {
                    lstData = db.DMATHANGs.Where(x => x.HANGSALE > 0);
                }
                else if (info.loaiMatHang == (int)LoaiHang.HangMoiVe)
                {
                    lstData = db.DMATHANGs.OrderBy(x => x.TIMECREATED);
                }
                else if (info.loaiMatHang == (int)LoaiHang.HangSapVe)
                {
                    lstData = db.DMATHANGs.OrderBy(x => x.DTHOIGIANDATID.Length > 0);
                }
            }
            if (info.dangId != null && info.dangId.Length > 0)
            {
                lstData = db.DMATHANGs.Where(x => info.dangId.Contains(x.DDANGID));
            }
            if (info.nhomId != null && info.nhomId.Length > 0)
            {
                lstData = db.DMATHANGs.Where(x => info.nhomId.Contains(x.DNHOMHANGID));
            }
            if (info.phanLoaiId != null && info.phanLoaiId.Length > 0)
            {
                lstData = db.DMATHANGs.Where(x => info.phanLoaiId.Contains(x.DPHANLOAIID));
            }
            if (info.mauId != null && info.mauId.Length > 0)
            {
                foreach (string itemMau in info.mauId)
                {
                    lstData = db.DMATHANGs.Where(x => x.DMAUCHITIETs.Any(a=>a.DMAUID == itemMau));
                }
            }
            if (info.sizeId != null && info.sizeId.Length > 0)
            {
                foreach (string itemSize in info.sizeId)
                {
                    lstData = db.DMATHANGs.Where(x => x.DSIZECHITIETs.Any(a => a.DSIZEID == itemSize));
                }
            }
            if (info.thoiGianId != null && info.thoiGianId.Length > 0)
            {
                lstData = db.DMATHANGs.Where(x => info.dangId.Contains(x.DDANGID));
            }
            lstData = lstData.OrderBy(x => x.NAME).Skip((info.page - 1) * pageSize).Take(pageSize);
            List<DMATHANG> lst = lstData.ToList();
            int loaiGia = LayLoaiGiaTheoKhach();
            foreach (DMATHANG item in lst)
            {
                item.GIABAN = LayGiaMatHangTheoKhachHang(item, loaiGia);
            }
            ViewBag.pageSize = pageSize;
            return PartialView(lst);
        }

        //Danh sách trang chủ
        public ActionResult MatHangTongHop(int? loai)
        {
            if (loai == null) return HttpNotFound();

            string tieuDe = "";
            List<DMATHANG> lst = null;
            if (loai == (int)LoaiHang.HangSale)
            {
                tieuDe = "Hàng Sale";
                lst = db.DMATHANGs.Where(x => x.HANGSALE > 0).ToList();
            }
            else if (loai == (int)LoaiHang.HangMoiVe)
            {
                tieuDe = "Hàng Mới Về";
                lst = db.DMATHANGs.OrderBy(x=>x.TIMECREATED).Take(20).ToList();
            }
            else if (loai == (int)LoaiHang.HangSapVe)
            {
                tieuDe = "Hàng Sắp Về";
                lst = db.DMATHANGs.OrderBy(x => x.DTHOIGIANDATID.Length > 0).Take(20).ToList();
            }

            //Đổi giá bán mặt hàng theo giá khách
            if (lst != null && lst.Count > 0)
            {
                int loaiGia = LayLoaiGiaTheoKhach();
                foreach (DMATHANG item in lst)
                {
                    item.GIABAN = LayGiaMatHangTheoKhachHang(item, loaiGia);
                }
            }

            ViewBag.TieuDe = tieuDe;
            return PartialView(lst);
        }

        public ActionResult ViewMatHang(string id)
        {
            DMATHANG mhRow = db.DMATHANGs.Find(id);
            return PartialView(mhRow);
        }

        public static int LayLoaiGiaTheoKhach()
        {
            UserModel userLogin = CookieUtils.UserLogin();
            int loaiGia = 0;
            if (userLogin.dKHACHHANG != null && userLogin.dKHACHHANG.DNHOMKHACHHANG != null)
            {
                loaiGia = userLogin.dKHACHHANG.DNHOMKHACHHANG.LOAIGIA ?? 0;
            }
            return loaiGia;
        }

        public static long LayGiaMatHangTheoKhachHang(DMATHANG item, int loaiGia)
        {
            long donGia = 0;
            if (loaiGia == 0)
            {
                donGia = item.GIABAN??0;
            } else if (loaiGia == 0)
            {
                donGia = item.GIABAN ?? 0;
            }
            else if (loaiGia == 1)
            {
                donGia = item.GIABAN2 ?? 0;
            }
            else if (loaiGia == 2)
            {
                donGia = item.GIABAN3 ?? 0;
            }
            else if (loaiGia == 3)
            {
                donGia = item.GIABAN4 ?? 0;
            }
            else if (loaiGia == 4)
            {
                donGia = item.GIABAN5 ?? 0;
            }
            else if (loaiGia == 5)
            {
                donGia = item.GIABAN6 ?? 0;
            }
            else if (loaiGia == 6)
            {
                donGia = item.GIABAN7 ?? 0;
            }
            else if (loaiGia == 7)
            {
                donGia = item.GIABAN8 ?? 0;
            }
            return donGia;
        }
    }

    public enum LoaiHang
    {
        HangSale = 0,
        HangMoiVe = 1,
        HangSapVe = 2
    }
}