using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.AppRegister.Models;
using log4net;
using Tw.Bus.IRepository;
using Tw.Bus.Cache;
using System.Linq.Expressions;
using Tw.Bus.Entity;
using Sakura.AspNetCore;
using Tw.Bus.EntityDTO;
using Tw.Bus.AppRegister.Filters;
using Tw.Bus.Common;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Tw.Bus.AppRegister.Controllers
{
    [TwBusAuthorize]
    public class HomeController : Controller
    {
        private readonly ILog log = null;

        private readonly IUsyAppRepository _appRepository;

        private readonly IRedisCacheService _redisCache;

        private readonly ICacheService _memoryCache;

        private readonly IHostingEnvironment _env;

        public HomeController(IUsyAppRepository appRepository, IRedisCacheService redisCache, ICacheService memoryCache, IHostingEnvironment env)
        {
            log = LogManager.GetLogger(Startup.log4netRepository.Name, typeof(HomeController));
            _appRepository = appRepository;
            _redisCache = redisCache;
            _memoryCache = memoryCache;
            _env = env;

        }
        public async Task<IActionResult> Index(int page = 1, string kword = "", int pageSize = 2)
        {

            //log.Info("GetCurrentDirectory:" + Directory.GetCurrentDirectory());
            //log.Info(_env.EnvironmentName);
            //log.Info("ContentRootPath:" + _env.ContentRootPath);

            List<AppDto> lst = new List<AppDto>();


            Expression<Func<Usy_App, bool>> filter = null;

            if (string.IsNullOrEmpty(kword) == false)
            {
                filter = t => t.AppName.Contains(kword);
            }

            Expression<Func<Usy_App, object>> order = t => t.AppId;

            var lstEntity = await _appRepository.GetListForPageAsync(page, pageSize, filter, order);

            lst = AutoMapper.Mapper.Map<List<AppDto>>(lstEntity);

            int totalCount = await _appRepository.GetCountAsync(filter);

            //计算共有多少页
            var pages = Convert.ToDouble(totalCount) / Convert.ToDouble(pageSize);

            pages = Math.Ceiling(pages);

            int totalPage = Convert.ToInt32(pages);

            var lstAppDtos = new PagedList<IEnumerable<AppDto>, AppDto>(lst, lst, pageSize, page, totalCount, totalPage);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", lstAppDtos);
            }
            else
            {
                return View(lstAppDtos);
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AppDto dto)
        {
            JsonModel jm = new JsonModel();


            Usy_App app = new Usy_App();
            app.AppName = dto.AppName;

            app.AppType = dto.AppType;

            app.AddTime = DateTime.Now;

            app.AppStatus = dto.AppStatus;

            app.AppId = Guid.NewGuid().ToString();

            string strDesKeys = "TwBusDes";

            app.AppKey = DESCrypHelper.Encrypt(strDesKeys, app.AppId);

            app = await _appRepository.InsertAsync(app);
            if (app.id > 0)
            {

                jm.Statu = "y";
                jm.Msg = "添加成功！";
            }
            else
            {
                jm.Statu = "n";
                jm.Msg = "执行失败，请稍后再试！";
            }

            return Json(jm);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Usy_App app = await _appRepository.GetAsync(id.Value);

            if (app == null)
            {
                return NotFound();
            }

            AppDto dto = AutoMapper.Mapper.Map<AppDto>(app);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppDto dto)
        {
            JsonModel jm = new JsonModel();

            dto.UpdateTime = DateTime.Now;

            Usy_App app= AutoMapper.Mapper.Map<Usy_App>(dto);

            app = await _appRepository.UpdateAsync(app);

            jm.Statu = "y";
            jm.Msg = "修改成功！";

            return Json(jm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            JsonModel jm = new JsonModel();

            if (id == null)
            {
                return BadRequest();
            }

            int nA = await _appRepository.DeleteAsync(id.Value);

            if (nA > 0)
            {
                jm.Statu = "y";
                jm.Msg = "删除成功！";
            }
            else
            {
                jm.Statu = "n";
                jm.Msg = "删除失败,请稍后再试！";
            }

            return Json(jm);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            throw new Exception("这是测试自定义错误！");
        }

    }
}
