using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authorization;

namespace Tw.Bus.WebApi.Filters
{
    /// <summary>
    /// 在swagger页面中提供头部信息输入参数
    /// </summary>
    public class HttpHeaderOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            var actionAttrs = context.ApiDescription.ActionAttributes();

            var isAuthorized = actionAttrs.Any(a => a.GetType() == typeof(AuthorizeAttribute));

            if (isAuthorized == false) //提供action都没有权限特性标记，检查控制器有没有
            {
                var controllerAttrs = context.ApiDescription.ControllerAttributes();

                isAuthorized = controllerAttrs.Any(a => a.GetType() == typeof(AuthorizeAttribute));
            }

            var isAllowAnonymous = actionAttrs.Any(a => a.GetType() == typeof(AllowAnonymousAttribute));

            if (isAuthorized && isAllowAnonymous == false)
            {
                operation.Parameters.Add(new NonBodyParameter()
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "string",
                    Required = false
                });
            }
        }
    }
}
