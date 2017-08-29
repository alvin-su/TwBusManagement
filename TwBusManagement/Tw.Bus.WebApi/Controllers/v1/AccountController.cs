using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using log4net;
using Tw.Bus.IRepository;
using Tw.Bus.Cache;
using Tw.Bus.WebApi.Models;
using Tw.Bus.EntityDTO;
using Tw.Bus.Entity;
using Tw.Bus.Common;
using Microsoft.AspNetCore.Authorization;

namespace Tw.Bus.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/account")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AccountController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(Startup.log4netRepository.Name, typeof(AccountController));

        private readonly IUsyUserRepository _userRepository;

        private readonly IRedisCacheService _redisCache;

        private readonly ICacheService _memoryCache;

        public AccountController(IUsyUserRepository userRepository, IRedisCacheService redisCache, ICacheService memoryCache)
        {
            _userRepository = userRepository;

            _redisCache = redisCache;
            _memoryCache = memoryCache;
        }

        
        [Route("SignIn")]
        [HttpPost, MapToApiVersion("1.0")]
        [Authorize]
        public async Task<string> SignIn([FromBody]LoginUser user)
        {
            UserDto dto = new UserDto();
            try
            {
                Usy_User entityUser = await _userRepository.FirstOrDefaultAsync(t =>  t.JobNumber == user.JobNumber && t.Pwd == user.Pwd);

                if (entityUser != null)
                {
                    dto = AutoMapper.Mapper.Map<UserDto>(entityUser);
                }
                else
                {
                    dto.IsError = true;
                    dto.ErrorMsg = "登录的账号或密码错误！";
                }
            }
            catch (Exception ex)
            {
                log.Error("SignIn方法错误", ex);
                dto.IsError = true;
                dto.ErrorMsg = ex.Message;
            }

            return JsonHelper.SerializeObject(dto);
        }

    }
}
