using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;

namespace Tw.Bus.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("hosting.json", optional: true)
               .Build();

            BuildWebHost(args,config).Run();
        }

        public static IWebHost BuildWebHost(string[] args,IConfigurationRoot config) =>

          WebHost.CreateDefaultBuilder(args)
              .UseStartup<Startup>()
              .UseConfiguration(config)
              .Build();
    }
}
