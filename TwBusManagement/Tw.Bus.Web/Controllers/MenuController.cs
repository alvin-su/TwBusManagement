using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Tw.Bus.Web.Common;
using Tw.Bus.Common;
using System.Net.Http;
using Tw.Bus.EntityDTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tw.Bus.Cache;

namespace Tw.Bus.Web.Controllers
{
    public class MenuController : BaseController
    {

        public MenuController(IOptions<ApiServer> option, IRedisCacheService redisCache, ICacheService memoryCache) :
            base(option, redisCache, memoryCache)
        {
            
        }

        public async Task<IActionResult> Index()
        {
           // bool b = await _redisCache.SetAsync<string>("TwBusWeb", "测试2222");

            string strApiUrl = ApiServerAddr + @"/api/v1/manage/querymenu";

            SearchMenuParamsDto paramsDto = new SearchMenuParamsDto();
            paramsDto.parentId = 0;
            paramsDto.lstRoleId = UserInfo.lstRoleID;

            string strJson = JsonHelper.SerializeObject(paramsDto);


            HttpContent content = new StringContent(strJson);

            string strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken);

            List<MenuDto> lstMenu = new List<MenuDto>();

            if (strRes.Contains("网络错误") == false)
            {
                lstMenu = JsonHelper.Deserialize<List<MenuDto>>(strRes);
            }

            return View(lstMenu);
        }
        [HttpGet]
        public async Task<IActionResult> Add(int? ParentId, int? MenuId)
        {
            JsonModel jm = new JsonModel();
            EntityDTO.MenuDto model = new MenuDto();
            try
            {
                //获取菜单下拉列表
                var menulist = new List<SelectListItem>();
                menulist.Add(new SelectListItem { Text = "顶级菜单", Value = "0" });
                //var list = client.Channel.GetListHaveSortForMenu(0);
                int _parentID = 0;
                GetMenuList(menulist, _parentID);
                if (menulist != null && menulist.Count != 0)
                {
                    ViewBag.MenuList = new SelectList(menulist, "Value", "Text", ParentId);
                }
                if (MenuId == null)//添加
                {
                    ViewBag.IsEdit = "false";
                }
                else//编辑
                {
                    ViewBag.IsEdit = "true";

                    string strApiUrl = ApiServerAddr + @"/api/v1/manage/MenuFindById";

                    string strJson = JsonHelper.SerializeObject(MenuId);


                    HttpContent content = new StringContent(strJson);

                   string strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken);

                    if (strRes.Contains("网络错误") == false)
                    {
                        model = JsonHelper.Deserialize<MenuDto>(strRes);
                    }
                    if (model == null)
                    {
                        jm.Msg = "参数错误,找不到需要编辑的菜单项";
                        jm.Statu = "error";
                        TempData["msg"] = jm;
                        return RedirectToAction("Index");
                    }
                }

            }
            catch
            {
                throw;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(MenuDto viewmodel)
        {
            EntityDTO.MenuDto model = new MenuDto();
            JsonModel rjson = new JsonModel();
            UserViewModel uinfo = UserInfo;
            try
            {
                string strApiUrl = ApiServerAddr + @"/api/v1/manage/MenuFindById";

                string strJson = JsonHelper.SerializeObject(viewmodel.ParentId);


                HttpContent content = new StringContent(strJson);

                string strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken);

                EntityDTO.MenuDto parentmodel = null;

                if (strRes.Contains("网络错误") == false)
                {
                    parentmodel = JsonHelper.Deserialize<MenuDto>(strRes);
                }

                if (parentmodel != null)
                {
                    model.Class_layer = parentmodel.Class_layer + 1;
                }
                else
                {
                    model.Class_layer = 1;
                }
                if (viewmodel.id != 0)
                {

                   strJson = JsonHelper.SerializeObject(viewmodel.id);

                    content = new StringContent(strJson);

                    strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken);

                    if (strRes.Contains("网络错误") == false)
                    {
                        model = JsonHelper.Deserialize<MenuDto>(strRes);
                    }

                    if (model == null)
                    {
                        rjson.Msg = "参数错误,找不到需要编辑的菜单项";
                        rjson.Statu = "y";
                        return Json(rjson);
                    }
                }
                model.ActionName = viewmodel.ActionName;
                model.Class_list = viewmodel.Class_list;
                model.ControllerName = viewmodel.ControllerName;
                model.IconCss = viewmodel.IconCss;
                model.IsLock = viewmodel.IsLock;
                model.LinkPara = viewmodel.LinkPara;
                model.Name = viewmodel.Name;
                model.ParentId = viewmodel.ParentId;
                model.Remark = viewmodel.Remark;
                model.SortID = viewmodel.SortID;
                model.UpdateTime = DateTime.Now;
                model.UpdateUser = uinfo.id.ToString();
                int res = 0;
                if (viewmodel.id == 0)
                {
                    model.AddTime = DateTime.Now;
                    model.AddUser = uinfo.id.ToString();

                    strApiUrl = ApiServerAddr + @"/api/v1/manage/MenuAdd";

                }
                else
                {
                    strApiUrl = ApiServerAddr + @"/api/v1/manage/MenuUpdate";

                }


                strJson = JsonHelper.SerializeObject(model);

                content = new StringContent(strJson);

                strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken);

                if (strRes.Contains("网络错误") == false)
                {
                    res = int.Parse(JsonHelper.Deserialize<string>(strRes));
                }

                if (res > 0)
                {
                    HttpContext.Session.Set<string>("MenuCustomId", Guid.NewGuid().ToString());

                    rjson.Msg = "提交成功";
                    rjson.Statu = "y";
                }
                else
                {
                    rjson.Msg = "提交失败";
                    rjson.Statu = "n";
                }

            }
            catch (Exception ex)
            {
                rjson.Msg = Utils.GetServiceErrMsg(ex);
                rjson.Statu = "n";
            }

            return Json(rjson);
        }

        /// <summary>
        /// 根据父ID获取菜单列表SelectList
        /// </summary>
        /// <param name="menulSelectList"></param>
        /// <param name="parentID"></param>
        public  void GetMenuList(List<SelectListItem> menulSelectList, int parentID)
        {
            IList<MenuDto> list = new List<MenuDto>();
            try
            {

                string strApiUrl = ApiServerAddr + @"/api/v1/manage/querymenu";

                SearchMenuParamsDto paramsDto = new SearchMenuParamsDto();
                paramsDto.parentId = parentID;
 
                string strJson = JsonHelper.SerializeObject(paramsDto);

                HttpContent content = new StringContent(strJson);

                string strRes = ApiHelp.ApiPostWithTokenAsync(strApiUrl, content, AccessToken).Result;


                if (strRes.Contains("网络错误") == false)
                {
                    list = JsonHelper.Deserialize<List<MenuDto>>(strRes);
                }


            }
            catch
            {
                throw;
            }
            foreach (var item in list)
            {
                SelectListItem selectitem = new SelectListItem();
                string name = item.Name;
                string id = item.id.ToString();
                if (item.Class_layer == parentID)
                {
                    menulSelectList.Add(new SelectListItem { Text = name, Value = id });
                }
                else
                {
                    name = "├ " + name;
                    name = Utils.StringOfChar(item.Class_layer - 1, "　") + name;
                    menulSelectList.Add(new SelectListItem { Text = name, Value = id });
                }
            }
        }

    }
}