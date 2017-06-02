using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient myHttpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"UserName","alvinsu"},
                {"Password","123456" }
             });

            //一定要设定Header
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


            myHttpClient.BaseAddress = new Uri("http://localhost:8002/");



            HttpResponseMessage response =  myHttpClient.PostAsync("api/authtoken", content).Result;

            string result = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
            }

            Console.ReadLine();

            // Console.WriteLine("Hello World!");
        }
    }
}