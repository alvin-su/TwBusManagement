using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tw.Bus.Common
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

    }
}
