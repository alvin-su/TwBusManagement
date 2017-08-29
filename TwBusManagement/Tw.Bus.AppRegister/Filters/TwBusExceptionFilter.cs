using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.AppRegister.Filters
{
    public class TwBusExceptionFilter : IExceptionFilter
    {
        private  ILog log;

        private readonly IHostingEnvironment _env;

        public TwBusExceptionFilter(IHostingEnvironment env)
        {

            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
           
           string str= context.HttpContext.Request.Path.Value;

           //string strController = str.Split("/")[1];

           // string strAction = str.Split("/")[2];

            log= LogManager.GetLogger(Startup.log4netRepository.Name, str);

            log.Error(context.Exception.Message, context.Exception);

        }
    }
}
