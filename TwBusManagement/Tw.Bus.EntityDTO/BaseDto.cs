using System;
using System.Collections.Generic;
using System.Text;

namespace Tw.Bus.EntityDTO
{
    public class BaseDto
    {
        public int id { get; set; }

        /// <summary>
        /// 是否错误
        /// </summary>
        public bool IsError { get; set; } = false;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }

    }
}
