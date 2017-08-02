using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.Web.Models
{
    /// <summary>
    /// AccessToken解析类
    /// </summary>
    public class AccessTokenModel
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }
    }
}
