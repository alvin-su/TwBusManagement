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

namespace Tw.Bus.WebApi.Controllers
{
    //[Produces("application/json")]
   
    public class ManageController : Controller
    {
        private readonly IUsyUserRepository _userRepository;

        public ManageController(IUsyUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 获取所有用户信息，加入Jwt认证（需先请求接口api/authtoken），需要在请求Headers 中加入 Authorization:Bearer 获取的Token值
        /// 还需在请求Headers的中加入 Content-Type:application/json
        /// </summary>
        /// <returns></returns>
        [Route("api/queryalluser")]
        [HttpGet]
        [Authorize]
        public async Task<string> GetAllUser()
        {
            var users = await _userRepository.GetAllListAsync();
            var dtos = AutoMapper.Mapper.Map<IEnumerable<UserDto>>(users);
            return Common.JsonHelper.SerializeObject(dtos);
        }
    }
}