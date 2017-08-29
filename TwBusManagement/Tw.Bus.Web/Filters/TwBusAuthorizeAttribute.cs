using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Tw.Bus.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;

namespace Tw.Bus.Web.Filters
{
    public class TwBusAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {

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

            return;
            //判断类或方法上是否应用了“AllowAnonymousAttribute”特性，该特性表示要跳过权限认证

            bool b = context.ActionDescriptor.FilterDescriptors.Any(t => t.Filter.ToString() == "Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter");
            if (b)
            {
                return;
            }


            string strControllerName = "";

            string strActionName = "";

            //获取控制器名称
            context.ActionDescriptor.RouteValues.TryGetValue("controller", out strControllerName);

            //获取操作名称
            context.ActionDescriptor.RouteValues.TryGetValue("action", out strActionName);

            string str = context.HttpContext.Request.Method;

        
          //  base.OnActionExecuting(filterContext);

        }
    }
}
