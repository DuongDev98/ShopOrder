using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using ShopOrder.Entities;
using ShopOrder.Models;

namespace ShopOrder.Utils
{
    public class FilterUser : ActionFilterAttribute, IAuthenticationFilter
    {
        ShopOrderEntities db = new ShopOrderEntities();
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            string controller = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            string action = filterContext.RouteData.Values["Action"].ToString().ToLower();

            if (!(controller == "user" && (action == "login" || action == "register")))
            {
                UserModel userLogin = CookieUtils.UserLogin();
                if (userLogin == null || userLogin.sUSER == null && userLogin.dKHACHHANG == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
                else
                {
                    bool noError = false;
                    //khách hàng không được vào admin
                    if (userLogin.IsCustomer)
                    {
                        if (controller == "user" && (action == "doimatkhau" || action == "thongtincanhan")) noError = true;
                        if (controller == "khachhang" && action == "edit") noError = true;
                        if (controller == "home" || controller == "cart") noError = true;
                        if (controller == "quanlydonhang") noError = true;
                    }
                    else
                    {
                        noError = true;
                        if ((controller == "home" && action != "index") || (controller == "cart" && action == "index")) noError = false;
                    }
                    if (!noError)
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            string controller = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            string action = filterContext.RouteData.Values["Action"].ToString().ToLower();

            if (controller == "home" && action == "index")
            {
                UserModel userLogin = CookieUtils.UserLogin();
                if (userLogin != null && !userLogin.IsCustomer)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Admin" }, { "action", "Index" } });
                }
            }

            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                        { "controller", "User" },
                        { "action", "Login" }
                   });
                //If you want to redirect to some error view, use below code
                //filterContext.Result = new ViewResult()
                //{
                //    ViewName = "Login"
                //};
            }
        }
    }
}