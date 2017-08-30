using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.AppRegister.Models;
using Tw.Bus.Common;
using Tw.Bus.EntityDTO;
using Tw.Bus.IRepository;
using log4net;
using Tw.Bus.AppRegister.Filters;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Tw.Bus.AppRegister.Controllers
{
    public class AccountController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(Startup.log4netRepository.Name, typeof(AccountController));

        private readonly IUsyUserRepository _userRepository;

        private readonly IHostingEnvironment _env;

        public AccountController(IUsyUserRepository userRepository, IHostingEnvironment env)
        {
            _userRepository = userRepository;
            _env = env;
        }

      //  [TwBusException]
        [HttpGet]
        public IActionResult Login()
        {
          
            //log.Info("GetCurrentDirectory:" + Directory.GetCurrentDirectory());
            //log.Info(_env.EnvironmentName);
            //log.Info("ContentRootPath:"+_env.ContentRootPath);

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
        public async Task<IActionResult> Login(ApplicationUser appUser)
        {

            JsonModel jm = new JsonModel();

            if (string.IsNullOrWhiteSpace(appUser.JobNumber.Trim()) || string.IsNullOrWhiteSpace(appUser.Password.Trim()))
            {
                jm.Statu = "n";
                jm.Msg = "工号、密码不能为空！";
                return Json(jm);
            }

            string md5Pwd = SecurityUtility.GetMd5Hash(appUser.Password.Trim()).ToUpper();

            appUser.Password = md5Pwd;

          

            UserDto model = null;
            UserViewModel vm = null;

            try
            {
                var entity = await _userRepository.GetUserByAccountAndPwdAsync(appUser.JobNumber, appUser.Password);

                // model = proxy.Channel.GetUserByAccountAndPwd(account, md5Pwd);
                if (entity == null) //登录失败
                {
                    jm.Statu = "n";
                    jm.Msg = "用户名或密码错误！";
                    return Json(jm);
                }
                else  //登录成功
                {

                    model = AutoMapper.Mapper.Map<UserDto>(entity);

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

                    return Json(jm);

                }

            }
            catch (Exception ex)
            {

                jm.Statu = "n";
                jm.Msg = ex.Message;
                // ZY.Edu.Common.LogHelper.WriteError("用户登录", ex);
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

    }
}