using ShopOrder.Entities;
using ShopOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopOrder.Utils
{
    public class UiUtils
    {
        public static string ImageQrCode(TDONHANG dhRow)
        {
            string number = "1012340416";
            string url = @"https://img.vietqr.io/image/{0}-{1}-print.jpg?amount={2}&addInfo={3}";
            url = string.Format(url, Banks.Vietcombank, number, dhRow.TONGCONG, HttpUtility.UrlEncode(dhRow.TMPCODE));
            return url;
        }

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

    public enum Banks
    {
        VietinBank,
        Vietcombank,
        Agribank,
        BIDV,
        Techcombank,
        ACB
    }
}