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
using Tw.Bus.Cache;
using Tw.Bus.Web.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Tw.Bus.Web.Controllers
{
    public class HomeController : BaseController
    {
     
        public HomeController(IOptions<ApiServer> option, IRedisCacheService redisCache, ICacheService memoryCache) :
            base(option, redisCache, memoryCache)
        {
          
        }

        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult About()
        {


            return View();
        }

    
        public IActionResult Error()
        {
            return View();
        }
    }
}
