using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tw.Bus.IRepository;
using Tw.Bus.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Tw.Bus.WebApi.JwtOptions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tw.Bus.WebApi.Filters;

namespace Tw.Bus.WebApi
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
            //添加 可配置功能
            services.AddOptions();

            var arrAllowSpecificOrigin = Configuration.GetSection("AllowSpecificOrigin")?.Value.Split(',');

            //添加跨域资源请求访问
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                  builder => builder.WithOrigins(arrAllowSpecificOrigin)
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .AllowCredentials());

                //options.AddPolicy("AllowAllOrigins",
                //      builder =>
                //      {
                //          builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                //      });


            });


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

                c.OperationFilter<HttpHeaderOperationFilter>(); // 添加httpHeader参数
            });

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            // 注释下面这句表示不启用全局CORS中间件，在控制器中单独实现，
            //可在控制器中添加 [EnableCors("AllowSpecificOrigin")] 特性
            app.UseCors("AllowSpecificOrigin");
            //app.UseCors("AllowAllOrigins");


           // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
