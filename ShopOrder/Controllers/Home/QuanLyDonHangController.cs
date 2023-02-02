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
    public class QuanLyDonHangController : Controller
    {
        private ShopOrderEntities db = new ShopOrderEntities();
        public ActionResult Index()
        {
            UserModel userLogin = CookieUtils.UserLogin();
            Dictionary<DTRANGTHAIDON, int> dic = new Dictionary<DTRANGTHAIDON, int>();
            Random rd = new Random();
            dic.Add(new DTRANGTHAIDON() { ID = "", NAME = "Chờ thanh toán" }, db.TDONHANGs.Count(x=>x.DATHANHTOAN != 30));
            var tmp = db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList();
            foreach (var item in tmp)
            {
                dic.Add(item, db.TDONHANGs.Count(x => x.DATHANHTOAN == 30 && x.DTRANGTHAIDONID == item.ID));
            }
            ViewBag.Layout = userLogin.IsCustomer ? "~/Views/Shared/_Layout.cshtml" : "~/Views/Shared/_Admin.cshtml";
            return View(dic);
        }

        public ActionResult DanhSach(string id)
        {
            string title = "";
            UserModel userLogin = CookieUtils.UserLogin();
            IQueryable<TDONHANG> result = null;
            if (id == null || id.Length == 0)
            {
                result = db.TDONHANGs.Where(x => x.DATHANHTOAN != 30);
                title = "Danh sách chờ thanh toán";
            }
            else
            {
                result = db.TDONHANGs.Where(x => x.DTRANGTHAIDONID == id);
                title = db.DTRANGTHAIDONs.Find(id).NAME;
            }

            if (userLogin.IsCustomer)
            {
                result = result.Where(x=>x.DKHACHHANGID == userLogin.dKHACHHANG.ID);
            }
            ViewBag.Title = title;
            ViewBag.Layout = userLogin.IsCustomer ? "~/Views/Shared/_Layout.cshtml" : "~/Views/Shared/_Admin.cshtml";
            return View(result.ToList());
        }
    }
}