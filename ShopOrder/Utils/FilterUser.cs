﻿using System;
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

            if (!(controller == "User".ToLower() && (action == "Login".ToLower() || action == "Register".ToLower())))
            {
                UserModel userLogin = CookieUtils.UserLogin();
                if (userLogin == null || userLogin.sUSER == null && userLogin.dKHACHHANG == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
                else
                {
                    //khách hàng không được vào admin
                    if (userLogin.IsCustomer)
                    {
                        bool noError = false;
                        if (controller == "User".ToLower() && (action == "DoiMatKhau".ToLower() || action == "ThongTinCaNhan".ToLower())) noError = true;
                        if (controller == "Khachhang".ToLower() && action == "Edit".ToLower()) noError = true;
                        if (controller == "Home".ToLower() || controller == "Cart".ToLower()) noError = true;
                        if (!noError)
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                        }
                    }
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //string controller = filterContext.RouteData.Values["Controller"].ToString();
            //string action = filterContext.RouteData.Values["Action"].ToString();

            //if (controller == "Admin")
            //{
            //    controller = "Home";
            //    action = "Index";
            //}
            //else
            //{
            //    controller = "User";
            //    action = "Login";
            //}

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