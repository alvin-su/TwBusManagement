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

namespace Tw.Bus.WebApi.Controllers
{
    //[Produces("application/json")]
   
    public class ManageController : Controller
    {

        private static readonly ILog log = LogManager.GetLogger(Startup.log4netRepository.Name, typeof(ManageController));

        private readonly IUsyUserRepository _userRepository;

        public ManageController(IUsyUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// ��ȡ�����û���Ϣ������Jwt��֤����������ӿ�api/authtoken������Ҫ������Headers �м��� Authorization:Bearer ��ȡ��Tokenֵ
        /// ����������Headers���м��� Content-Type:application/json
        /// </summary>
        /// <returns></returns>
        [Route("api/queryalluser")]
        [HttpPost]
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

            var users = await _userRepository.GetAllListAsync();
            var dtos = AutoMapper.Mapper.Map<IEnumerable<UserDto>>(users);
            return Common.JsonHelper.SerializeObject(dtos);
        }



    }
}