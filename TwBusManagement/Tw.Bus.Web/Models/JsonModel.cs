using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.Web.Models
{
    public class JsonModel
    {
        public object Data { get; set; }
        public string Msg { get; set; }
        public string Statu { get; set; }
        public string BackUrl { get; set; }

        public void GetJsonMsg(int res)
        {
            if (res > 0)
            {
                this.Msg = "操作执行成功";
                this.Statu = "y";
            }
            else if (res < 0)
            {
                this.Msg = "操作执行失败";
                this.Statu = "y";
            }
            else
            {
                this.Msg = "数据未做任何修改";
                this.Statu = "w";
            }
        }
    }
}
