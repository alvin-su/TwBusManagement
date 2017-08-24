using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tw.Bus.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.Cache;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.Memory;

namespace Tw.Bus.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();

            services.Configure<ApiServer>(Configuration.GetSection("ApiServer"));

            services.AddMemoryCache();

            services.AddMvc();

            //添加Redis分布式缓存
            services.AddSingleton(typeof(IRedisCacheService), new RedisCacheService(new RedisCacheOptions
            {
                Configuration = Configuration.GetConnectionString("RedisConnection"),
                InstanceName = "TwBus"

            }, 0));

            //添加内存缓存
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            services.AddSingleton<ICacheService, MemoryCacheService>();

            services.AddSession(configure =>
            {
                configure.IdleTimeout = new TimeSpan(0, 180, 0); //session过期时间
            });

          
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "TwBusMenu_";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

           // app.UseResponseCaching();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
