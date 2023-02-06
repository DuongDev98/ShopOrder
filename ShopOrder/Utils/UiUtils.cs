using ShopOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopOrder.Utils
{
    public class UiUtils
    {
        public static string Layout(UserModel userLogin)
        {
            return userLogin.IsCustomer ? "~/Views/Shared/_Layout.cshtml" : "~/Views/Shared/_Admin.cshtml";
        }

        public static string Layout()
        {
            UserModel userLogin = CookieUtils.UserLogin();
            return userLogin.IsCustomer ? "~/Views/Shared/_Layout.cshtml" : "~/Views/Shared/_Admin.cshtml";
        }
    }
}