using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.Web.Models
{
    public class LoginUser
    {
        /// <summary>
        /// 工号(登录账号)
        /// </summary>
        public string JobNumber { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
    }
}
