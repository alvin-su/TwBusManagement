using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Tw.Bus.AppRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Tw.Bus.AppRegister.Filters
{
    public class TwBusAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // throw new NotImplementedException();


            string strUrl = "";


            strUrl = context.HttpContext.Request.Path.Value;

            if (context.HttpContext.Request.QueryString.HasValue)
            {
                strUrl += context.HttpContext.Request.QueryString.Value;
            }

            #region Session代码
            if (context.HttpContext.Session.Get<UserViewModel>("UserInfo") == null)  //没有登录系统直接访问控制器
            {
                //filterContext.Result = new RedirectResult("/Account/Login");

                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    var data = new JsonModel { Statu = "y", BackUrl = strUrl };
                    context.Result = new JsonResult(data);

                }
                else
                {
                    context.Result =
                     new RedirectToRouteResult
                     (
                       new RouteValueDictionary
                       (
                         new
                         {
                             controller = "Account",
                             action = "Login",
                             area = "",
                             ReturnUrl = strUrl,
                         }
                       )
                     );
                }
                return;
            }
            #endregion

        }
    }
}
