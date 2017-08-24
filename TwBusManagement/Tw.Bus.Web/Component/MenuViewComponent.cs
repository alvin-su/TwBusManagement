using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tw.Bus.EntityDTO;
using Tw.Bus.Web.Common;
using Tw.Bus.Web.Models;
using Tw.Bus.Common;

namespace Tw.Bus.Web.Component
{
    public class MenuViewComponent: ViewComponent
    {

        public string ApiServerAddr { get; private set; }

        public MenuViewComponent(IOptions<ApiServer> option)
        {
            ApiServerAddr = option.Value.Addr;
        }

      
        public async Task<IViewComponentResult> InvokeAsync()
        {

            //var list =  await Task<List<MenuDto>>.Run(()=> {
            //    return _menuService.MenuGetListHaveSort(0, false, rolelist).Where(c => c.IsLock == 0).ToList();
            //});
            var user = HttpContext.Session.Get<UserViewModel>("UserInfo");
           // List<int> rolelist = user.lstRoleID;
            var list = await GetMenuAsync(user);
            //HttpContext.Session.Set<string>("MenuCustomId", DateTime.Now.ToString("yyyyMMddHHmmss"));
            return View("_LeftMenu", list);


        }

        private async Task<List<MenuDto>> GetMenuAsync(UserViewModel user)
        {
            string strToken = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(strToken))
            {
                //获取访问token
                AccessTokenModel tokenModel = await ApiHelp.GetAccessTokenAsync(user.JobNumber, user.Pwd, ApiServerAddr);

                HttpContext.Session.SetString("token", tokenModel.access_token);

                strToken = tokenModel.access_token;
            }

            string strApiUrl = ApiServerAddr + @"/api/v1/manage/querymenu";


            SearchMenuParamsDto paramsDto = new SearchMenuParamsDto();
            paramsDto.parentId = 0;
            paramsDto.lstRoleId = user.lstRoleID;

            string strJson = JsonHelper.SerializeObject(paramsDto);

            HttpContent content = new StringContent(strJson);

            string strRes = await ApiHelp.ApiPostWithTokenAsync(strApiUrl, content,strToken);

            List<MenuDto> lstMenu = new List<MenuDto>();

            if (strRes.Contains("网络错误") == false)
            {
                lstMenu = JsonHelper.Deserialize<List<MenuDto>>(strRes);
            }

            return lstMenu;
            //return Task.FromResult(_menuService.MenuGetListHaveSort(0, true, rolelist).Where(c => c.IsLock == 0).ToList());
        }
    }
}
