using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.Web.Models;
using Tw.Bus.Common;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Tw.Bus.Web.Common;
using Tw.Bus.EntityDTO;
using Tw.Bus.Cache;
using Tw.Bus.Web.Filters;
using Microsoft.AspNetCore.Http;

namespace Tw.Bus.Web.Controllers
{
    public class AccountController : Controller
    {
        public string ApiServerAddr { get; private set; }

        public string AppId { get; private set; }

        public string AppKey { get; private set; }

        public AccountController(IOptions<ApiServer> option)
        {
            ApiServerAddr = option.Value.Addr;
            AppId = option.Value.AppId;
            AppKey = option.Value.AppKey;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.Get<UserViewModel>("UserInfo") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {

                return View();
            }
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUser loginVm)
        {

            JsonModel jm = new JsonModel();

            if (string.IsNullOrWhiteSpace(loginVm.JobNumber.Trim()) || string.IsNullOrWhiteSpace(loginVm.Pwd.Trim()))
            {
                jm.Statu = "n";
                jm.Msg = "工号、密码不能为空！";
                return Json(jm);
            }

            string md5Pwd = SecurityUtility.GetMd5Hash(loginVm.Pwd.Trim()).ToUpper();

            loginVm.Pwd = md5Pwd;

            string strApiUrl = ApiServerAddr + @"/api/v1/account/SignIn";

            string strJson = JsonHelper.SerializeObject(loginVm);


            HttpContent content = new StringContent(strJson);

            UserDto model = null;
            UserViewModel vm = null;

            try
            {
                string strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken);

                if (strRes.Contains("网络错误") == false)
                {
                   
                    model = JsonHelper.Deserialize<UserDto>(strRes);

                    if (model.IsError)
                    {
                        jm.Statu = "n";
                        jm.Msg = model.ErrorMsg;
                        return Json(jm);
                    }


                    if (model.Status == 1)
                    {
                        jm.Statu = "n";
                        jm.Msg = "你的账号已被冻结！";
                        return Json(jm); //还未审核的用户
                    }
                    vm = new UserViewModel();
                    vm.id = model.id;
                    vm.UserName = model.UserName;
                    vm.JobNumber = model.JobNumber;
                    vm.Pwd = model.Pwd;
                    //vm. = model.Sex;

                    //vm.s = model.SchoolID;
                    vm.Status = model.Status;
                    if (model.Status == 0)
                    {
                        vm.StatuName = "已启用";
                    }
                    else
                    {
                        vm.StatuName = "已冻结";
                    }

                    vm.lstRoleID = model.LstRoleID;

                    vm.LastLoginTime = model.LastLoginTime;
                    vm.RegisterTime = model.RegisterTime;
                    vm.Remark = model.Remark;

                    HttpContext.Session.Set<Models.UserViewModel>("UserInfo", vm);
                    jm.Statu = "y";
                    jm.Msg = "登录成功";
                  
                }
                else
                {
                    jm.Statu = "n";
                    jm.Msg = "网络错误，请确认网络连接!";
                }

                return Json(jm);
            }
            catch (Exception ex)
            {
                jm.Statu = "n";
                jm.Msg = ex.Message;
                return Json(jm);
            }
           

        }

        [HttpPost]
        public ActionResult Logout()//退出
        {
            
            #region 非统一认证注销
            HttpContext.Session.Set<UserViewModel>("UserInfo", null);
            HttpContext.Session.Clear();

            return Json(1);
            #endregion

        }

        public string AccessToken
        {
            get
            {
                return GetTokenAsync().Result;
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