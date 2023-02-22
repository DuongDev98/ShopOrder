using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            IQueryable<TDONHANG> donHangs = db.TDONHANGs.Where(x => x.LOAI == 0);

            //khách hàng chỉ hiện thị đơn của nó
            if (userLogin.IsCustomer) donHangs = donHangs.Where(x => x.DKHACHHANGID == userLogin.dKHACHHANG.ID);

            dic.Add(new DTRANGTHAIDON() { ID = "", NAME = "Chờ thanh toán" }, donHangs.Count(x => x.DATHANHTOAN != 30));

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
            ViewBag.Title = "Danh sách chi tiết";
            if (id != null && id.Length > 0)
            {
                UserModel userLogin = CookieUtils.UserLogin();
                DTRANGTHAIDON tRANGTHAIDON = db.DTRANGTHAIDONs.Find(id);
                if (tRANGTHAIDON == null) return HttpNotFound();
                IQueryable<TDONHANGCHITIET> donHangChiTiets = db.TDONHANGCHITIETs;
                if (userLogin.IsCustomer) donHangChiTiets = donHangChiTiets.Where(x => x.DKHACHHANGID == userLogin.dKHACHHANG.ID);
                donHangChiTiets = donHangChiTiets.Where(x => x.DTRANGTHAIDONID == id);
                List<TDONHANGCHITIET> lst = donHangChiTiets.ToList();

                ViewBag.Title = "Danh sách " + tRANGTHAIDON.NAME;
                ViewBag.Layout = UiUtils.Layout(userLogin);

                if (lst.Count == 0)
                {
                    return RedirectToAction("Index", "QuanLyDonHang");
                }
                else
                {
                    ViewBag.IsCustomer = userLogin.IsCustomer;
                    ViewBag.ViewDaNhan = Config.DATA.TTXACNHANDANHANID == id;
                    return View("DanhSachChiTiet", lst.ToList());
                }
            }
            else
            {
                return RedirectToAction("DanhSachChuaThanhToan", "QuanLyDonHang");
            }
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
            ViewBag.ViewDaNhan = Config.DATA.TTXACNHANDANHANID == id;
            return View(result.ToList());
        }

        public ActionResult DanhSachChuaThanhToan()
        {
            UserModel userLogin = CookieUtils.UserLogin();
            IQueryable<TDONHANG> result = db.TDONHANGs.Where(x => x.LOAI == 0 && x.DATHANHTOAN != 30);
            if (userLogin.IsCustomer)
            {
                result = result.Where(x => x.DKHACHHANGID == userLogin.dKHACHHANG.ID);
            }
            ViewBag.IsCustomer = userLogin.IsCustomer;
            ViewBag.Title = "Danh sách chưa thanh toán";
            ViewBag.Layout = UiUtils.Layout(userLogin);
            return View(result.ToList());
        }

        public ActionResult DoiTrangThaiThanhToan(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Find(id);
            if (dhRow == null) return HttpNotFound();
            if (dhRow.DATHANHTOAN != 30)
            {
                dhRow.TIENTHANHTOAN = dhRow.TONGCONG;
                dhRow.DATHANHTOAN = 30;
                db.Entry(dhRow);
                db.SaveChanges();
                DoiTrangThaiSauThanhToan(id);
                //log
                DatabaseUtils.Log(dhRow, "Đã thanh toán");
            }
            return RedirectToAction("DanhSach", "QuanLyDonHang");
        }

        public ActionResult DoiTrangThaiDaNhan(string id)
        {
            TDONHANGCHITIET ctRow = db.TDONHANGCHITIETs.Find(id);
            if (ctRow == null) return HttpNotFound();
            SCONFIG sconfig = Config.DATA;

            string lastId = "";
            if (sconfig != null && sconfig.TTXACNHANDANHANID != null && sconfig.TTXACNHANDANHANID.Length > 0)
            {
                if (lastId.Length == 0) lastId = ctRow.DTRANGTHAIDONID;
                ctRow.DTRANGTHAIDONID = sconfig.TTDANHANID;
                db.Entry(ctRow);
                db.SaveChanges();
                //log
                DatabaseUtils.Log(ctRow, "Đã nhận hàng");
            }
            return RedirectToAction("DanhSach", "QuanLyDonHang", new { id = lastId });
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

        [HttpPost]
        public ActionResult DichVuChuyenTien(ThueApiModel model)
        {
            bool kq = false;
            //kiểm tra token xem được gửi từ thue api ko
            if (Request.Headers.AllKeys.Contains("x-thueapi"))
            {
                string tokenReceive = Request.Headers.Get("x-thueapi");
                if (tokenReceive == "YCI3sLMtTgWmUnWsRDdfoOwK447IzXDECCDodZvgqEdISqV4j6")
                {
                    kq = true;
                }
            }
            if (!kq)
            {
                return Content("error");
            }
            if (model == null || model.txn_id == null) return Content("error");
            TDONHANG dhRow = db.TDONHANGs.Where(x => model.content.Contains(x.TMPCODE)).FirstOrDefault();
            if (dhRow == null) return Content("error");
            else
            {
                dhRow.TIENTHANHTOAN = (long)Convert.ToDecimal(model.money);
                if (dhRow.TIENTHANHTOAN >= dhRow.TONGCONG)
                {
                    dhRow.DATHANHTOAN = 30;
                    db.Entry(dhRow);
                    db.SaveChanges();
                    DoiTrangThaiSauThanhToan(dhRow.ID);
                    //log
                    DatabaseUtils.Log(dhRow, "Đã thanh toán");
                    return Content("ok");
                }
                else
                {
                    return Content("error");
                }
            }
        }

        private void DoiTrangThaiSauThanhToan(string iD)
        {
            SCONFIG sCONFIG = Config.DATA;
            if (sCONFIG != null && sCONFIG.TTSAUTHANHTOANID != null && sCONFIG.TTSAUTHANHTOANID.Length > 0)
            {
                List<TDONHANGCHITIET> chiTiets = db.TDONHANGCHITIETs.Where(x => x.TDONHANGID == iD).ToList();
                foreach (TDONHANGCHITIET ctRow in chiTiets)
                {
                    ctRow.DTRANGTHAIDONID = sCONFIG.TTSAUTHANHTOANID;
                    db.Entry(ctRow);
                }
                db.SaveChanges();
            }
        }

        //public ActionResult CapTrangThaiThanhToan()
        //{
        //    List<TDONHANG> lst = db.TDONHANGs.Where(x => x.DATHANHTOAN != 30).ToList();
        //    foreach (TDONHANG item in lst)
        //    {
        //        //thực hiện truy vấn kiểm tra cú pháp tin nhắn có tồn tại không
        //        bool kq = false;
        //        if (kq)
        //        {
        //            item.DATHANHTOAN = 30;
        //            db.Entry(item);
        //            db.SaveChanges();
        //            //log
        //            DatabaseUtils.Log(item, "Đã thanh toán");
        //        }
        //    }
        //    return RedirectToAction("DanhSachChiTiet", "QuanLyDonHang");
        //}
    }
}