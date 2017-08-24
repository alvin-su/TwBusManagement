using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.Web.Models
{
    [ProtoContract]
    public class UserViewModel
    {
        [ProtoMember(1)]
        public int id { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>	
        [ProtoMember(2)]
        public string UserName { get; set; }
        /// <summary>
        /// 工号(登录账号)
        /// </summary>
        [ProtoMember(3)]
        public string JobNumber { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [ProtoMember(4)]
        public string Pwd { get; set; }
        /// <summary>
        /// 用户状态0=启用 1=禁用
        /// </summary>		
        [ProtoMember(5)]
        public int Status { get; set; } = 0;
        /// <summary>
        /// 注册时间
        /// </summary>	
        [ProtoMember(6)]
        public DateTime? RegisterTime { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>	
        [ProtoMember(7)]
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [ProtoMember(8)]
        public string Remark { get; set; }

        /// <summary>
        /// 所拥有的角色
        /// </summary>	
        [ProtoMember(9)]
        public List<int> lstRoleID { get; set; } = new List<int>();

        public string StatuName { get; set; }
    }
}
