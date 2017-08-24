using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.IRepository;
using Tw.Bus.EntityFrameworkCore;
using Tw.Bus.Entity;
using Tw.Bus.EntityDTO;
using Tw.Bus.Common;
using Microsoft.AspNetCore.Authorization;
using Tw.Bus.WebApi.Filters;
using Tw.Bus.WebApi.Models;
using log4net;
using Tw.Bus.Cache;

namespace Tw.Bus.WebApi.Controllers.v1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/manage")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ManageController : Controller
    {

        private static readonly ILog log = LogManager.GetLogger(Startup.log4netRepository.Name, typeof(ManageController));

        private readonly IUsyUserRepository _userRepository;

        private readonly IRedisCacheService _redisCache;

        private readonly ICacheService _memoryCache;

        private readonly IUsyMenuRepository _menuRepository;

        public ManageController(IUsyUserRepository userRepository, IRedisCacheService redisCache, ICacheService memoryCache, IUsyMenuRepository menuRepository)
        {
            _userRepository = userRepository;

            _redisCache = redisCache;
            _memoryCache = memoryCache;

            _menuRepository = menuRepository;
        }

       

        /// <summary>
        /// ��ȡ�����û���Ϣ������Jwt��֤����������ӿ�api/authtoken������Ҫ������Headers �м��� Authorization:Bearer ��ȡ��Tokenֵ
        /// ����������Headers���м��� Content-Type:application/json
        /// </summary>
        /// <returns></returns>
        [Route("queryalluser")]
        [HttpPost, MapToApiVersion("1.0")]
        [Authorize]
        public async Task<string> GetAllUser()
        {
            try
            {
                throw new Exception("���Դ���");
            }
            catch (Exception ex)
            {
                log.Error("GetAllUser��������",ex);
            }

           bool b=await _redisCache.SetAsync<string>("test","����");

            var users = await _userRepository.GetAllListAsync();
            var dtos = AutoMapper.Mapper.Map<IEnumerable<UserDto>>(users);
            return Common.JsonHelper.SerializeObject(dtos);
        }
        [Route("querymenu")]
        [HttpPost, MapToApiVersion("1.0")]
        [Authorize]
        public async Task<string> GetMenuGetListHaveSort([FromBody]SearchMenuParamsDto dto)
        {
            try
            {
                var list = await _menuRepository.GetListHaveSortAsync(dto.parentId, dto.IsShowHide, dto.lstRoleId);

                var dtos= AutoMapper.Mapper.Map<List<MenuDto>>(list);

                return Common.JsonHelper.SerializeObject(dtos);
            }
            catch (Exception ex)
            {
                log.Error("GetMenuGetListHaveSort��������", ex);
                throw;
            }
        }
        [Route("MenuFindById")]
        [HttpPost, MapToApiVersion("1.0")]
        [Authorize]
        public async Task<string> GetMenuById([FromBody]int menuid)
        {
            try
            {
                Usy_Menu menu=await _menuRepository.GetAsync(menuid);

                var dto = AutoMapper.Mapper.Map<MenuDto>(menu);

                return Common.JsonHelper.SerializeObject(dto);
            }
            catch (Exception ex)
            {
                log.Error("GetMenuById��������", ex);
                throw;
            }
        }
        [Route("MenuAdd")]
        [HttpPost, MapToApiVersion("1.0")]
        [Authorize]
        public async Task<string> MenuAdd([FromBody]MenuDto dto)
        {
            try
            {
                Usy_Menu model = AutoMapper.Mapper.Map<Usy_Menu>(dto);

                model = await _menuRepository.InsertAsync(model);

                return Common.JsonHelper.SerializeObject(model.Id);

                //return _menuRepository.InsertAsync(model).Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Route("MenuUpdate")]
        [HttpPost, MapToApiVersion("1.0")]
        [Authorize]
        public async Task<string> MenuUpdate([FromBody]MenuDto dto)
        {
            try
            {
                Usy_Menu model = AutoMapper.Mapper.Map<Usy_Menu>(dto);

                model = await _menuRepository.UpdateAsync(model);

                return Common.JsonHelper.SerializeObject(model.Id);

               
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}