using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    public class AdminController : BaseController
    {
        ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            //DatabaseUtils.GetTable("");
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
            ViewBag.Id = user.sUSER.ID;
            ViewBag.Name = name;
            ViewBag.Username = username;
            return PartialView("NavbarAdminUi");
        }
    }
}