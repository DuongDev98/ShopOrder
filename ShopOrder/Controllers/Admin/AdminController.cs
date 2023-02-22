using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    public class AdminController : BaseController
    {
        ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NavbarAdminUi()
        {
            UserModel user = CookieUtils.UserLogin();
            string name = "", username = "";
            if (user.IsAdmin)
            {
                name = "Administrator";
            }
            else
            {
                name = user.sUSER.DNHANVIEN.NAME;
                username = user.sUSER.USERNAME;
            }
            ViewBag.ID = user.sUSER.ID;
            ViewBag.Name = name;
            ViewBag.Username = username;
            return PartialView("NavbarAdminUi");
        }

        [HttpGet]
        public ActionResult CauHinhThongBao()
        {
            ViewBag.data = HttpUtility.HtmlDecode(Config.DATA.THONGBAO);
            return View();
        }

        [HttpPost]
        public ActionResult CauHinhThongBao(string THONGBAO)
        {
            if (THONGBAO == null) return HttpNotFound();
            Config.setThongBao(THONGBAO);
            ViewBag.data = HttpUtility.HtmlDecode(THONGBAO);
            return View();
        }

        [HttpGet]
        public ActionResult CauHinhThongTin()
        {
            ViewBag.data = HttpUtility.HtmlDecode(Config.DATA.THONGTIN);
            return View();
        }

        [HttpPost]
        public ActionResult CauHinhThongTin(string THONGTIN)
        {
            if (THONGTIN == null) return HttpNotFound();
            Config.setThongTin(THONGTIN);
            ViewBag.data = HttpUtility.HtmlDecode(THONGTIN);
            return View();
        }

        [HttpGet]
        public ActionResult CauHinhCuoiTrang()
        {
            ViewBag.data = HttpUtility.HtmlDecode(Config.DATA.NOIDUNGCHANTRAN);
            return View();
        }

        [HttpPost]
        public ActionResult CauHinhCuoiTrang(string NOIDUNGCHANTRAN)
        {
            if (NOIDUNGCHANTRAN == null) return HttpNotFound();
            Config.setFooterContent(NOIDUNGCHANTRAN);
            ViewBag.data = HttpUtility.HtmlDecode(NOIDUNGCHANTRAN);
            return View();
        }

        [HttpGet]
        public ActionResult CauHinhTrangThai()
        {
            SCONFIG cf = Config.DATA;
            ViewBag.TTSAUTHANHTOANID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", cf.TTSAUTHANHTOANID);
            ViewBag.TTTAODIEUPHOIID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", cf.TTTAODIEUPHOIID);
            ViewBag.TTDANHANID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", cf.TTDANHANID);
            ViewBag.TTXACNHANDANHANID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", cf.TTXACNHANDANHANID);
            return View();
        }

        [HttpPost]
        public ActionResult CauHinhTrangThai(SCONFIG tmp)
        {
            bool addNew = false;
            SCONFIG data;
            if (tmp.ID == null)
            {
                addNew = true;
                data = new SCONFIG();
                data.ID = Guid.NewGuid().ToString();
            }
            else
            {
                data = db.SCONFIGs.FirstOrDefault();
            }

            data.TTDANHANID = tmp.TTDANHANID;
            data.TTSAUTHANHTOANID = tmp.TTSAUTHANHTOANID;
            data.TTTAODIEUPHOIID = tmp.TTTAODIEUPHOIID;
            data.TTXACNHANDANHANID = tmp.TTXACNHANDANHANID;
            if (addNew) db.SCONFIGs.Add(data);
            else db.Entry(data);
            db.SaveChanges();
            tmp.ID = data.ID;
            ViewBag.TTSAUTHANHTOANID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", tmp.TTSAUTHANHTOANID);
            ViewBag.TTTAODIEUPHOIID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", tmp.TTTAODIEUPHOIID);
            ViewBag.TTDANHANID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", tmp.TTDANHANID);
            ViewBag.TTXACNHANDANHANID = new SelectList(db.DTRANGTHAIDONs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", tmp.TTXACNHANDANHANID);
            return View();
        }

        [HttpGet]
        public ActionResult CauHinhGHTK()
        {
            return View(Config.DATA);
        }

        [HttpPost]
        public ActionResult CauHinhGHTK(SCONFIG tmp)
        {
            bool addNew = false;
            SCONFIG data;
            if (tmp.ID == null)
            {
                addNew = true;
                data = new SCONFIG();
                data.ID = Guid.NewGuid().ToString();
            }
            else
            {
                data = db.SCONFIGs.FirstOrDefault();
            }
            data.PICK_NAME = tmp.PICK_NAME;
            data.PICK_PROVINCE = tmp.PICK_PROVINCE;
            data.PICK_DISTRICT = tmp.PICK_DISTRICT;
            data.PICK_WARD = tmp.PICK_WARD;
            data.PICK_ADDRESS = tmp.PICK_ADDRESS;
            data.PICK_TEL = tmp.PICK_TEL;
            data.PICK_EMAIL = tmp.PICK_EMAIL;
            if (addNew) db.SCONFIGs.Add(data);
            else db.Entry(data);
            db.SaveChanges();
            tmp.ID = data.ID;
            return View(tmp);
        }
    }
}