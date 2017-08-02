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

            #region 获取Access_token
            ApplicationUser user = new ApplicationUser
            {
                UserName = "admin",
                Password = "123456"
            };
            string strJsonUser = JsonHelper.SerializeObject(user);

            string strJwtCry = Common.JwtCryptHelper.EncodeByJwt(strJsonUser);

            HttpContent content = new StringContent(strJwtCry);

            string strUrl = ApiServerAddr + @"/api/v1/jwt/token";

            string result = await ApiHelp.ApiPostAsync(strUrl, content);

            AccessTokenModel tokenModel = JsonHelper.Deserialize<AccessTokenModel>(result);
            #endregion

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
