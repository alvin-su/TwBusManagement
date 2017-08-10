using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tw.Bus.Common;
using Tw.Bus.Web.Models;

namespace Tw.Bus.Web.Common
{
    public class ApiHelp
    {
        public static async Task<string> ApiPostAsync(string url, HttpContent content)
        {
            using (var http = new HttpClient())
            {
                //设定Header
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await http.PostAsync(url, content);

                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "网络错误，请确认网络连接";
                }

            }

        }

        public static async Task<string> ApiPostWithTokenAsync(string url, HttpContent content, string strToken)
        {
            using (var http = new HttpClient())
            {
                //设定Header
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), url);


                request.Headers.Add("Authorization", "Bearer " + strToken);

                request.Content = content;


                HttpResponseMessage response = await http.SendAsync(request);

                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {

                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "网络错误，请确认网络连接";
                }
            }

        }
        /// <summary>
        /// 获取AccessToken的方法
        /// </summary>
        /// <param name="user">当前登录用户</param>
        /// <param name="strApiServerAddr">Api接口地址</param>
        /// <returns></returns>
        public static async Task<AccessTokenModel> GetAccessTokenAsync(ApplicationUser user,string strApiServerAddr)
        {
            string strJsonUser = JsonHelper.SerializeObject(user);

            string strJwtCry = Tw.Bus.Common.JwtCryptHelper.EncodeByJwt(strJsonUser);

            HttpContent content = new StringContent(strJwtCry);

            string strUrl = strApiServerAddr + @"/api/v1/jwt/token";

            string result = await ApiPostAsync(strUrl, content);

            AccessTokenModel tokenModel = JsonHelper.Deserialize<AccessTokenModel>(result);

            return tokenModel;
        }
    }
}
