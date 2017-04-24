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
        /// 得到所有用户信息
        /// </summary>
        /// <returns></returns>
        [Route("api/GetAllUser")]
        [HttpGet]
        public async Task<string> GetAllUser()
        {
            var users = await _userRepository.GetAllListAsync();

            var dtos = AutoMapper.Mapper.Map<IEnumerable<UserDto>>(users);
            return Common.JsonHelper.SerializeObject(dtos);
        }
    }
}