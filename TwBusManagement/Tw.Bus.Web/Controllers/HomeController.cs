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
using System.Security.Principal;

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
         
            UserViewModel user = new UserViewModel();
            user.id = 1;
            user.UserName = "admin";
            user.Pwd = "123456";
            user.JobNumber = "1001";
            user.lstRoleID.Add(1);

            HttpContext.Session.Set<UserViewModel>("UserInfo", user);

            

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
           AccessTokenModel tokenModel= await ApiHelp.GetAccessTokenAsync("admin","123456", ApiServerAddr);


            string strApiUrl = ApiServerAddr + @"/api/v1/manage/queryalluser";
        
          
            string strJson = "";

            HttpContent content = new StringContent(strJson);

            string strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content,tokenModel.access_token);

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
