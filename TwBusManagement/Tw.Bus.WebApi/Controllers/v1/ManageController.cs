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
    //[Produces("application/json")]
    
    [ApiVersion("1.0")]
    public class ManageController : Controller
    {

        private static readonly ILog log = LogManager.GetLogger(Startup.log4netRepository.Name, typeof(ManageController));

        private readonly IUsyUserRepository _userRepository;

        private readonly IRedisCacheService _redisCache;

        private readonly ICacheService _memoryCache;

        public ManageController(IUsyUserRepository userRepository, IRedisCacheService redisCache, ICacheService memoryCache)
        {
            _userRepository = userRepository;

            _redisCache = redisCache;
            _memoryCache = memoryCache;
        }

        [HttpGet, MapToApiVersion("1.0")]
        public IActionResult Get()
        {
            return Json(new string[] { "value1", "value2" });
        }

        /// <summary>
        /// ��ȡ�����û���Ϣ������Jwt��֤����������ӿ�api/authtoken������Ҫ������Headers �м��� Authorization:Bearer ��ȡ��Tokenֵ
        /// ����������Headers���м��� Content-Type:application/json
        /// </summary>
        /// <returns></returns>
        [Route("api/queryalluser")]
        [HttpPost]
        [Authorize]
        [MapToApiVersion("1.0")]
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



    }
}