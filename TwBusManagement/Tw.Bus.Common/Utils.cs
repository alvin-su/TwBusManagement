using System;
using System.Collections.Generic;
using System.Text;

namespace Tw.Bus.Common
{
    public class Utils
    {
        public static string SelectSplitString = "#$#";
        #region 生成指定长度的字符串
        /// <summary>
        /// 生成指定长度的字符串,即生成strLong个str字符串
        /// </summary>
        /// <param name="strLong">生成的长度</param>
        /// <param name="str">以str生成字符串</param>
        /// <returns></returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }

            return ReturnStr;
        }

        #region 获取正确的错误信息（来自控制器还是服务层）
        public static string GetServiceErrMsg(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }
            else
            {
                return ex.InnerException.Message;
            }
        }
        #endregion
        #endregion
    }
}
