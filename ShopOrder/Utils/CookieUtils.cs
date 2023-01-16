using Newtonsoft.Json;
using ShopOrder.Entities;
using ShopOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace ShopOrder.Utils
{
    public class CookieUtils
    {
        const string GIOHANG = "Items";
        const string USER = "User";

        public static UserModel UserLogin()
        {
            ShopOrderEntities db = new ShopOrderEntities();
            UserModel model = JsonConvert.DeserializeObject<UserModel>(Get(USER));
            if (model != null)
            {
                DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.USERNAME == model.USERNAME && x.PASSWORD == model.PASSWORD).FirstOrDefault();
                if (khRow == null)
                {
                    SUSER userRow = db.SUSERs.Where(x => x.USERNAME == model.USERNAME && x.PASSWORD == model.PASSWORD).FirstOrDefault();
                    if (userRow != null) model.sUSER = userRow;
                }
                else
                {
                    model.dKHACHHANG = khRow;
                }
            }
            return model;
        }

        public static void LuuTaiKhoanDangNhap(UserModel newItem)
        {
            AddOrEdit(USER, JsonConvert.SerializeObject(newItem));
        }

        public static void XoaDuLieuDangNhap()
        {
            Remove(USER);
        }

        public static void ThemGioHang(ItemModel newItem)
        {
            List<ItemModel> lst = LayGioHang();
            //kiểm tra giỏ nếu tồn tại thì cộng số lượng, chưa thì thêm mới
            int index = -1;
            foreach (ItemModel it in lst)
            {
                if (it.DMATHANGID == newItem.DMATHANGID && it.DSIZEID == newItem.DSIZEID && it.DMAUID == newItem.DMAUID
                    && it.DONGIA == newItem.DONGIA)
                {
                    index = lst.IndexOf(it);
                    break;
                }
            }
            if (index == -1)
            {
                lst.Add(newItem);
            }
            else
            {
                lst[index].SOLUONG = lst[index].SOLUONG + newItem.SOLUONG;
                lst[index].THANHTIEN = lst[index].SOLUONG * lst[index].DONGIA;
            }
            AddOrEdit(GIOHANG, JsonConvert.SerializeObject(lst));
        }

        public static void ThayDoiSoLuong(ItemModel newItem)
        {
            List<ItemModel> lst = LayGioHang();
            //kiểm tra giỏ nếu tồn tại thì cộng số lượng, chưa thì thêm mới
            foreach (ItemModel it in lst)
            {
                if (it.DMATHANGID == newItem.DMATHANGID && it.DSIZEID == newItem.DSIZEID && it.DMAUID == newItem.DMAUID
                    && it.DONGIA == newItem.DONGIA)
                {
                    it.SOLUONG = newItem.SOLUONG;
                    it.THANHTIEN = it.DONGIA * it.SOLUONG;
                    break;
                }
            }
            AddOrEdit(GIOHANG, JsonConvert.SerializeObject(lst));
        }

        public static List<ItemModel> LayGioHang()
        {
            string data = Get(GIOHANG);
            List<ItemModel> lst = null;
            if (data == null || data.Length == 0)
            {
                lst = new List<ItemModel>();
            }
            else
            {
                lst = JsonConvert.DeserializeObject<List<ItemModel>>(data);
            }
            return lst;
        }

        public static void XoaGioHang()
        {
            Remove(GIOHANG);
        }

        private static void AddOrEdit(string name, string value)
        {
            value = HttpContext.Current.Server.UrlEncode(value);
            HttpCookie httpCookie = null;
            if (ReadCookies().AllKeys.Contains(name))
            {
                httpCookie = ReadCookies().Get(name);
                httpCookie.Value = value;
                SetCookies().Set(httpCookie);
            }
            else
            {
                httpCookie = new HttpCookie(name, value);
                httpCookie.Expires = DateTime.Now.AddMonths(1);
                SetCookies().Add(httpCookie);
            }
        }

        private static string Get(string name)
        {
            HttpCookie httpCookie = ReadCookies().Get(name);
            string value = httpCookie == null ? "" : httpCookie.Value;
            value = HttpContext.Current.Server.UrlDecode(value);
            return value;
        }

        private static void Remove(string name)
        {
            HttpCookie ck = ReadCookies().Get(name);
            if (ck != null)
            {
                ck.Expires = DateTime.Now.AddDays(-100);
                SetCookies().Set(ck);
            }
        }

        private static HttpCookieCollection ReadCookies()
        {
            return HttpContext.Current.Request.Cookies;
        }

        private static HttpCookieCollection SetCookies()
        {
            return HttpContext.Current.Response.Cookies;
        }
    }
}