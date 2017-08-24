using System;
using System.Collections.Generic;

namespace Tw.Bus.EntityDTO
{
    public class UserDto:BaseDto
    {
       // public int Id { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>		
        public string UserName { get; set; }
        /// <summary>
        /// 工号(登录账号)
        /// </summary>		
        public string JobNumber { get; set; }
        /// <summary>
        /// 密码  
        /// </summary>		
        public string Pwd { get; set; }
        /// <summary>
        /// 用户状态0=启用 1=禁用
        /// </summary>		
        public int Status { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>		
        public DateTime RegisterTime { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>		
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remark { get; set; }


        ///// <summary>
        ///// 用户所拥有的角色
        ///// </summary>
        public List<int> LstRoleID { get; set; } = new List<int>();


    }
}
