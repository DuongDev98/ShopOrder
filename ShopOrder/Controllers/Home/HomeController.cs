using ShopOrder.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    public class HomeController : Controller
    {
        ShopOrderEntities db = new ShopOrderEntities();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        //Danh sách trang chủ
        public ActionResult DanhSachMatHang(int? loai)
        {
            string tieuDe = "";
            List<DMATHANG> lst = null;

            if (loai == null || loai == 0)
            {
                tieuDe = "Hàng Sale";
                lst = db.DMATHANGs.Where(x => x.HANGSALE != null && x.HANGSALE > 0).ToList();
            }
            else if (loai == 1)
            {
                tieuDe = "Hàng Mới Về";
                lst = db.DMATHANGs.OrderBy(x=>x.TIMECREATED).Take(20).ToList();
            }
            else if (loai == 2)
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
            return 0;
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
}