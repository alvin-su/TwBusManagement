using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.Web.Models;
using Microsoft.AspNetCore.Http;
using Tw.Bus.Web.Common;
using Microsoft.Extensions.Options;
using Tw.Bus.Cache;
using Tw.Bus.Web.Filters;

namespace Tw.Bus.Web.Controllers
{
    [TwBusAuthorize]
    public class BaseController : Controller
    {
        public string ApiServerAddr { get; private set; }

        public string AppId { get; private set; }

        public string AppKey { get; private set; }

        public readonly IRedisCacheService _redisCache;

        public readonly ICacheService _memoryCache;

        public BaseController(IOptions<ApiServer> option, IRedisCacheService redisCache, ICacheService memoryCache)
        {
            ApiServerAddr = option.Value.Addr;
            AppId = option.Value.AppId;
            AppKey = option.Value.AppKey;
            _redisCache = redisCache;
            _memoryCache = memoryCache;
        }

        ///// <summary>
        ///// 登陆用户信息
        ///// </summary>
        public UserViewModel UserInfo
        {
            get
            {
                if (HttpContext.Session.Get<UserViewModel>("UserInfo") != null)
                {
                    return HttpContext.Session.Get<UserViewModel>("UserInfo");
                }
                else
                {
                    UserViewModel uinfo = new UserViewModel();
                    return uinfo;
                }
            }
        }

        public string AccessToken
        {
            get
            {
                return  GetTokenAsync().Result;
            }
        }
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenAsync()
        {
           
            string strToken = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(strToken))
            {
                //获取访问token
                AccessTokenModel tokenModel = await ApiHelp.GetAccessTokenAsync(AppId, AppKey, ApiServerAddr);

                HttpContext.Session.SetString("token", tokenModel.access_token);

                strToken = tokenModel.access_token;
            }
            return strToken;

        }

    }
}