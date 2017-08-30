using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using log4net.Repository;
using AutoMapper;
using log4net;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using log4net.Config;
using Tw.Bus.Entity;
using Tw.Bus.EntityDTO;
using Tw.Bus.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tw.Bus.Cache;
using Microsoft.Extensions.Caching.Redis;
using Tw.Bus.IRepository;
using Sakura.AspNetCore.Mvc;
using Tw.Bus.AppRegister.Filters;
using Microsoft.Extensions.FileProviders;

namespace Tw.Bus.AppRegister
{
    public class Startup
    {
        public static ILoggerRepository log4netRepository { get; set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
#if DEBUG
            env.EnvironmentName = EnvironmentName.Development;
#else
            env.EnvironmentName = EnvironmentName.Production;
#endif

            env.ContentRootPath = PlatformServices.Default.Application.ApplicationBasePath;

            Configuration = configuration;

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();

            

            //初始化实体映射关系
            Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<Usy_User, UserDto>().
                //ForMember(des => des.UserId, options => options.MapFrom(src => src.Id))
                //.ForMember(des => des.UserName, options => options.MapFrom(src => src.Name));

                cfg.CreateMap<Usy_App, AppDto>();
                cfg.CreateMap<AppDto, Usy_App>();

                cfg.CreateMap<Usy_User, UserDto>();
                cfg.CreateMap<UserDto, Usy_User>();

            });

            log4netRepository = LogManager.CreateRepository("NETCoreRepository");

            string strPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log4net.config");

            XmlConfigurator.Configure(log4netRepository, new FileInfo(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log4net.config")));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加 可配置功能
            services.AddOptions();

            //依赖注入
            AddDependencies(services);

            //添加数据上下文到连接池
            services.AddDbContextPool<TwBusDbContext>(options => options.UseMySql(Configuration.GetConnectionString("TwBusDbContext")));

            services.AddMemoryCache();

            // services.AddMvc();

            services.AddMvc(options =>
            {
                options.Filters.Add<TwBusExceptionFilter>();
            });

            ////添加Redis分布式缓存
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


            //Sakura.AspNetCore.Mvc
            services.AddBootstrapPagerGenerator(options =>
            {
                // Use default pager options.
                options.ConfigureDefault();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            //下面代码 发布到 linux centos7时需要取消注释
            //var staticfile = new StaticFileOptions();
            //string wwwrootPath= Path.Combine(env.ContentRootPath, "wwwroot");
            //staticfile.FileProvider = new PhysicalFileProvider(wwwrootPath);
            //app.UseStaticFiles(staticfile);

            //下面代码发布到 linux centos7时需要注释并用上面代码替换
            app.UseStaticFiles();

            app.UseSession();

            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        /// <summary>
        /// 注册依赖注入方法
        /// </summary>
        /// <param name="services"></param>
        private void AddDependencies(IServiceCollection services)
        {

            services.AddScoped<IUsyAppRepository, UsyAppRepository>();

            services.AddScoped<IUsyUserRepository, UsyUserRepository>();

        }
    }
}
