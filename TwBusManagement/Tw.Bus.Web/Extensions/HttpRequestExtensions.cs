using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.Web
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request.Headers != null)
            {
                //string str = request.Headers["X-Requested-With"].ToString();
                return request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest";
            }
            return false;
        }
    }
}
