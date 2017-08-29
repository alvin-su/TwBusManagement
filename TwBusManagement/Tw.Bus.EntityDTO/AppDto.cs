using System;
using System.Collections.Generic;
using System.Text;

namespace Tw.Bus.EntityDTO
{
    public class AppDto:BaseDto
    {
        /// <summary>
		/// 应用名称
        /// </summary>		
        public string AppName { get; set; }
        /// <summary>
        /// 由系统生成，作为应用的唯一标识
        /// </summary>		
        public string AppId { get; set; }
        /// <summary>
        /// 由系统生成，用来验证应用合法性的加密串
        /// </summary>		
        public string AppKey { get; set; }
        /// <summary>
        /// 应用状态 默认0=未审核，1=审核
        /// </summary>		
        public int AppStatus { get; set; } = 0;
        /// <summary>
        /// 应用类型
        /// </summary>		
        public string AppType { get; set; }
        /// <summary>
        /// AddTime
        /// </summary>		
        public DateTime? AddTime { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>		
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remark { get; set; }
    }
}
