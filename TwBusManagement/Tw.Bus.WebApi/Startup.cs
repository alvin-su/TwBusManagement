using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tw.Bus.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tw.Bus.IRepository;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Tw.Bus.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            //初始化映射关系
            TwBusDataMapper.InitializeDto();

        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加 可配置功能
            services.AddOptions();

            //依赖注入
            AddDependencies(services);

            //添加数据上下文
            services.AddDbContext<TwBusDbContext>(options => options.UseMySql(Configuration.GetConnectionString("TwBusDbContext")));        

            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "TwBusManagement接口文档",
                    Description = "RESTful API for TwBusManagement",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Alvin_Su", Email = "alvin_su@outlook.com", Url = "" }
                });

                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "twbusapi.xml");
                c.IncludeXmlComments(xmlPath);

               // c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TwBusManagement API V1");
                c.ShowRequestHeaders();
            });

            app.UseMvc();
        }

        /// <summary>
        /// 注册依赖注入方法
        /// </summary>
        /// <param name="services"></param>
        private void AddDependencies(IServiceCollection services)
        {

            services.AddTransient<IUsyUserRepository, UsyUserRepository>();
           
        }
    }
}
