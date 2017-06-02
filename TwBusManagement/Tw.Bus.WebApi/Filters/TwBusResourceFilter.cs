using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Common;
using Tw.Bus.WebApi.Models;

namespace Tw.Bus.WebApi.Filters
{
    public class TwBusResourceFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
           // throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            try
            {
                string str = string.Empty;

                using (Stream stream = context.HttpContext.Request.Body)
                {
                    StreamReader reader = new StreamReader(stream);
                    str = reader.ReadToEnd();
                }
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }

                str = JwtCryHelper.DecodeByJWT(str).ToString();

                str = str.Replace("\\", "");

                str = str.Remove(0, 1);

                str = str.Remove(str.Length - 1, 1);

                byte[] array = Encoding.ASCII.GetBytes(str);

                MemoryStream memoryStream = new MemoryStream(array);

                context.HttpContext.Request.Body = memoryStream;

                memoryStream.Flush();
              
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
