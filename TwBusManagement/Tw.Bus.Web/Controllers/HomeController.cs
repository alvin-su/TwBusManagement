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
using Microsoft.Extensions.Options;
using Tw.Bus.Web.Common;

namespace Tw.Bus.Web.Controllers
{
    public class HomeController : Controller
    {
        public string ApiServerAddr { get; private set; }

        public HomeController(IOptions<ApiServer> option)
        {
            ApiServerAddr = option.Value.Addr;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {

            ApplicationUser user = new ApplicationUser
            {
                UserName = "admin",
                Password = "123456"
            };
           //获取访问token
           AccessTokenModel tokenModel= await ApiHelp.GetAccessTokenAsync(user, ApiServerAddr);

            ViewData["Message"] = tokenModel.access_token;

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
