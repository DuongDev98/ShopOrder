using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers
{
    [FilterUser]
    public class BaseController : Controller
    {
    }
}