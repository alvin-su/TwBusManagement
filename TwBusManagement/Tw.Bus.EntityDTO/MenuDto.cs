using System;
using System.Collections.Generic;
using System.Text;

namespace Tw.Bus.EntityDTO
{
    public class MenuDto:BaseDto
    {
      //  public int id { get; set; }
        /// <summary>
        /// 菜单名
        /// </summary>		
        public string Name { get; set; }
        /// <summary>
        /// 控制器名
        /// </summary>		
        public string ControllerName { get; set; }
        /// <summary>
        /// 操作名
        /// </summary>		
        public string ActionName { get; set; }
        /// <summary>
        /// 菜单传递的参数(如:?id=1&&type=0)
        /// </summary>		
        public string LinkPara { get; set; }
        /// <summary>
        /// 排序号，默认值=0
        /// </summary>		
        public int SortID { get; set; }
        /// <summary>
        /// 是否显示菜单默认0=显示 1=不显示
        /// </summary>		
        public int IsLock { get; set; }
        /// <summary>
        /// 父菜单ID
        /// </summary>		
        public int ParentId { get; set; }
        /// <summary>
        /// 子菜单ID列表(逗号分隔)
        /// </summary>		
        public string Class_list { get; set; }
        /// <summary>
        /// 菜单所属层级
        /// </summary>		
        public int Class_layer { get; set; }
        /// <summary>
        /// 菜单Icon图标CSS类名
        /// </summary>		
        public string IconCss { get; set; }
        /// <summary>
        /// AddUser
        /// </summary>		
        public string AddUser { get; set; }
        /// <summary>
        /// AddTime
        /// </summary>		
        public DateTime? AddTime { get; set; }
        /// <summary>
        /// UpdateUser
        /// </summary>		
        public string UpdateUser { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>		
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// Remark
        /// </summary>		
        public string Remark { get; set; }
    }
}
