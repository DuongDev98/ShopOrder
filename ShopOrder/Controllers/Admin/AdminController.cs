using ShopOrder.Utils;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    public class AdminController : BaseController
    {
        public ActionResult Index()
        {
            DatabaseUtils.GetTable("");
            return View();
        }
    }
}