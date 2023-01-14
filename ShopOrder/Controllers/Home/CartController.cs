using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers.Home
{
    public class CartController : Controller
    {
        ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            List<ItemModel> lst = CookieUtils.LayGioHang();
            return View(lst);
        }

        public ActionResult AddToCart(ItemModel item)
        {
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
                item.DMATHANG_NAME = db.DMATHANGs.Find(item.DMATHANGID).NAME;
                item.DSIZE_NAME = db.DSIZEs.Find(item.DSIZEID).NAME;
                item.DMAU_NAME = db.DMAUs.Find(item.DMAUID).NAME;
                item.THANHTIEN = item.DONGIA * item.SOLUONG;
                CookieUtils.ThemGioHang(item);
            }
            return Content(error.Length == 0 ? "ok" : error);
        }

        public ActionResult ThayDoiSoLuong(ItemModel item)
        {
            CookieUtils.ThayDoiSoLuong(item);
            return Content("ok");
        }
    }
}