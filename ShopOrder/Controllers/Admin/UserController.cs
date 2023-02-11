using Microsoft.Ajax.Utilities;
using ShopOrder.Controllers.Model;
using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    public class UserController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        [HttpGet]
        public ActionResult Login()
        {
            CookieUtils.XoaDuLieuDangNhap();
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                string error = "";
                //khách hàng và nhân viên
                SUSER userRow = null;
                string passwordMd5 = PasswordUtils.EncrytPass(password);
                DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.USERNAME == username && x.PASSWORD == passwordMd5).FirstOrDefault();
                if (khRow == null)
                {
                    userRow = db.SUSERs.Where(x => x.USERNAME == username && x.PASSWORD == passwordMd5).FirstOrDefault();
                    if (userRow == null)
                    {
                        error = "Tài khoản hoặc mật khẩu không chính xác";
                    }
                }

                if (error.Length > 0)
                {
                    ViewBag.errorMessage = error;
                }
                else
                {
                    string controller = "";
                    UserModel user = new UserModel();
                    if (userRow == null)
                    {
                        //khách hàng
                        controller = "Home";
                        user.USERNAME = khRow.USERNAME;
                        user.PASSWORD = khRow.PASSWORD;
                    }
                    else
                    {
                        //Nhân viên
                        controller = "Admin";
                        user.USERNAME = userRow.USERNAME;
                        user.PASSWORD = userRow.PASSWORD;
                    }
                    CookieUtils.LuuTaiKhoanDangNhap(user);
                    return RedirectToAction("Index", controller);
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string phone, string name, string password, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                string error = "";
                if (password != confirmPassword)
                {
                    error = "Xác nhận mật khẩu không trùng khớp";
                }
                else
                {
                    //tên đăng nhập phải viết liền không dấu
                    if (username != PasswordUtils.RemoveUnicodeAndSpace(username))
                    {
                        error = "Tên đăng nhập phải viết liền không dấu";
                    }
                    else
                    //kiểm tra số điện thoại đúng định dạng
                    if (!NumberPhoneIsValid(phone))
                    {
                        error = "Số điện thoại không đúng định dạng";
                    }
                    else if (db.DKHACHHANGs.Where(x => x.USERNAME == username).ToList().Count > 0)
                    {
                        error = "Tài khoản đã tồn tại trong hệ thống";
                    }
                    else if (db.SUSERs.Where(x => x.USERNAME == username).ToList().Count > 0)
                    {
                        error = "Tài khoản đã tồn tại trong hệ thống";
                    }
                    else if (db.DKHACHHANGs.Where(x => x.NAME == name).ToList().Count > 0)
                    {
                        error = "Tên khách đã tồn tại trong hệ thống";
                    }
                    else if (db.DKHACHHANGs.Where(x => x.DIENTHOAI == phone).ToList().Count > 0)
                    {
                        error = "Số điện thoại đã tồn tại trong hệ thống";
                    }
                }
                if (error.Length > 0)
                {
                    ViewBag.errorMessage = error;
                }
                else
                {
                    DKHACHHANG khRow = new DKHACHHANG();
                    khRow.ID = Guid.NewGuid().ToString();
                    khRow.USERNAME = username;
                    khRow.PASSWORD = PasswordUtils.EncrytPass(password);
                    khRow.NAME = name;
                    khRow.DIENTHOAI = phone;
                    db.DKHACHHANGs.Add(khRow);
                    db.SaveChanges();
                    ViewBag.message = "Đăng ký tài khoản thành công";
                }
            }
            ViewBag.username = username;
            ViewBag.phone = phone;
            ViewBag.name = name;
            return View();
        }

        static string[] viettels = new string[] { "032", "033", "034", "035", "036", "037", "038", "039" };
        static string[] mobis = new string[] { "070", "076", "077", "078", "079" };
        static string[] vinas = new string[] { "081", "082", "083", "084", "085" };
        static string[] vietnams = new string[] { "056", "058" };
        static string[] gmobiles = new string[] { "059" };
        public static bool NumberPhoneIsValid(string phone)
        {
            if (phone.Length != 10) return false;
            foreach (char c in phone)
            {
                if (!Char.IsNumber(c)) return false;
            }
            foreach (string key in viettels)
            {
                if (phone.StartsWith(key)) return true;
            }
            foreach (string key in mobis)
            {
                if (phone.StartsWith(key)) return true;
            }
            foreach (string key in vinas)
            {
                if (phone.StartsWith(key)) return true;
            }
            foreach (string key in vietnams)
            {
                if (phone.StartsWith(key)) return true;
            }
            foreach (string key in gmobiles)
            {
                if (phone.StartsWith(key)) return true;
            }
            return false;
        }

        public ActionResult TaoTaiKhoan(string DNHANVIENID)
        {
            DNHANVIEN nvRow = db.DNHANVIENs.Find(DNHANVIENID);
            if (nvRow == null) return HttpNotFound();
            if (nvRow.SUSERs != null && nvRow.SUSERs.Count > 0)
            {
                return HttpNotFound("Nhân viên đã có tài khoản trong hệ thống");
            }
            return View("", "", DNHANVIENID);
        }

        [HttpPost]
        public ActionResult TaoTaiKhoan(string DNHANVIENID, string username, string password, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                DNHANVIEN nvRow = db.DNHANVIENs.Find(DNHANVIENID);
                if (nvRow == null) return HttpNotFound();
                if (nvRow.SUSERs != null && nvRow.SUSERs.Count > 0)
                {
                    return HttpNotFound("Nhân viên đã có tài khoản trong hệ thống");
                }

                string error = "";
                if (password != confirmPassword)
                {
                    error = "Nhập lại mật khẩu không trùng khớp";
                }
                else if (db.DKHACHHANGs.Where(x => x.USERNAME == username).ToList().Count > 0)
                {
                    error = "Tài khoản đã tồn tại trong hệ thống";
                }
                else if (db.SUSERs.Where(x => x.USERNAME == username).ToList().Count > 0)
                {
                    error = "Tài khoản đã tồn tại trong hệ thống";
                }

                if (error.Length > 0)
                {
                    ViewBag.errorMessage = error;
                }
                else
                {
                    SUSER userRow = new SUSER();
                    userRow.ID = Guid.NewGuid().ToString();
                    userRow.USERNAME = username;
                    userRow.PASSWORD = PasswordUtils.EncrytPass(password);
                    userRow.DNHANVIENID = DNHANVIENID;
                    userRow.ISADMIN = 0;
                    db.SUSERs.Add(userRow);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Nhanvien");
                }
            }
            return View("", "", DNHANVIENID);
        }

        public ActionResult DoiMatKhauNv(string DNHANVIENID)
        {
            DNHANVIEN nvRow = db.DNHANVIENs.Find(DNHANVIENID);
            if (nvRow == null || nvRow.SUSERs == null || nvRow.SUSERs.Count == 0) return HttpNotFound();
            return RedirectToAction("DoiMatKhau", "User", new { ID = nvRow.SUSERs.ElementAt(0).ID, KhachHang = false });
        }

        public ActionResult DoiMatKhau(string id, bool khachHang)
        {
            UserModel userLogin = CookieUtils.UserLogin();
            if (!UserLoginIsValid(id, khachHang, userLogin)) return HttpNotFound();
            ViewBag.Layout = UiUtils.Layout(userLogin);
            ViewBag.ID = id;
            ViewBag.KhachHang = khachHang;
            return View();
        }

        private bool UserLoginIsValid(string id, bool khachHang, UserModel userLogin)
        {
            if (khachHang)
            {
                DKHACHHANG khRow = db.DKHACHHANGs.Find(id);
                if (khRow == null) return false;

                //so sánh với tài khoản đăng nhập khách hàng
                if (khRow.USERNAME != userLogin.USERNAME) return false;
            }
            else
            {
                SUSER userRow = db.SUSERs.Find(id);
                if (userRow == null) return false;
                if (!userLogin.IsAdmin)
                {
                    //so sánh với tài khoản đăng nhập nhân viên
                    if (userRow.USERNAME != userLogin.USERNAME) return false;
                }
            }
            return true;
        }

        [HttpPost]
        public ActionResult DoiMatKhau(string id, bool khachHang, string oldPassword, string password, string confirmPassword)
        {
            UserModel userLogin = CookieUtils.UserLogin();
            if (ModelState.IsValid)
            {
                if (!UserLoginIsValid(id, khachHang, userLogin)) return HttpNotFound();

                string error = "";
                if (password != confirmPassword)
                {
                    error = "Nhập lại mật khẩu không trùng khớp";
                }
                else if (((userLogin.IsAdmin && userLogin.sUSER.ID == id) ||!userLogin.IsAdmin) && userLogin.PASSWORD != PasswordUtils.EncrytPass(oldPassword))
                {
                    error = "Mật khẩu cũ không đúng";
                }
                else if (password == oldPassword && !userLogin.IsAdmin)
                {
                    error = "Mật khẩu mới phải khác mật khẩu cũ";
                }

                if (error.Length > 0)
                {
                    ViewBag.errorMessage = error;
                }
                else
                {
                    string controller = "User";
                    if (khachHang)
                    {
                        DKHACHHANG khRow = db.DKHACHHANGs.Find(id);
                        khRow.PASSWORD = PasswordUtils.EncrytPass(password);
                        db.Entry(khRow);
                    }
                    else
                    {
                        SUSER userRow = db.SUSERs.Find(id);
                        userRow.PASSWORD = PasswordUtils.EncrytPass(password);
                        db.Entry(userRow);

                        if (userLogin.IsAdmin && id != userLogin.sUSER.ID)
                        {
                            controller = "Nhanvien";
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction(controller == "Nhanvien" ? "Index" : "Login", controller);
                }
            }

            ViewBag.Layout = UiUtils.Layout();
            ViewBag.ID = id;
            ViewBag.KhachHang = khachHang;
            return View();
        }

        public ActionResult Logout()
        {
            CookieUtils.XoaDuLieuDangNhap();
            return RedirectToAction("Index", "Admin");
        }
    }
}