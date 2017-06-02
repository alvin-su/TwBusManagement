using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Tw.Bus.Common;
using Tw.Bus.Web.Models;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace Tw.Bus.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";

           

            //post 提交 先创建一个和webapi对应的类
           

            ApplicationUser user = new ApplicationUser
            {
                UserName = "alvinsu",
                Password = "123456"
            };

            //设置HttpClientHandler的AutomaticDecompression
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            HttpClient myHttpClient = new HttpClient(handler);

            string strJsonUser = JsonHelper.SerializeObject(user);

            string strJwtCry = Common.JwtCryHelper.EncodeBYJWT(strJsonUser);

            HttpContent content = new StringContent(strJwtCry);

            //一定要设定Header
          
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


            myHttpClient.BaseAddress = new Uri("http://localhost:8002/");

            

            HttpResponseMessage response = await myHttpClient.PostAsync("api/authtoken", content);

            string result = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
            }
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
