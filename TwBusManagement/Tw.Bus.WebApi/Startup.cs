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
using log4net.Config;
using log4net;
using log4net.Repository;
using Tw.Bus.Cache;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tw.Bus.WebApi
{
    public class Startup
    {
        public static ILoggerRepository log4netRepository { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //初始化映射关系
            TwBusDataMapper.InitializeDto();


            log4netRepository = LogManager.CreateRepository("NETCoreRepository");

            // string strLog4netFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log4net.config");

            // XmlConfigurator.Configure(log4netRepository, new FileInfo("log4net.config"));

            string strPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log4net.config");

            XmlConfigurator.Configure(log4netRepository, new FileInfo(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log4net.config")));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加 可配置功能
            services.AddOptions();

            //添加跨域资源请求访问
             var arrAllowSpecificOrigin = Configuration.GetSection("AllowSpecificOrigin")?.Value.Split(',');

            services.AddCors(options =>
            {
                //options.AddPolicy("AllowSpecificOrigin",
                //  builder => builder.WithOrigins(arrAllowSpecificOrigin)
                //                       .AllowAnyHeader()
                //                       .AllowAnyMethod()
                //                       .AllowCredentials());

                options.AddPolicy("AllowAllOrigins",
                      builder =>
                      {
                          builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });


            });


            //依赖注入
            AddDependencies(services);



            //添加数据上下文
            services.AddDbContext<TwBusDbContext>(options => options.UseMySql(Configuration.GetConnectionString("TwBusDbContext")));



            services.AddMemoryCache();

            // Add framework services.
            services.AddMvc(c =>
               c.Conventions.Add(new ApiExplorerGroupPerVersionConvention())
            );


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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "TwBus Api v1",
                    Title = "TwBusManagement接口文档",
                    Description = "RESTful API for TwBusManagement",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Alvin_Su", Email = "alvin_su@outlook.com", Url = "" }
                });

                c.SwaggerDoc("v2", new Info
                {
                    Version = "TwBus Api v2",
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


            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Add JWT　Protection
            var secretKey = jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)];
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];

                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                // options.ValidFor = TimeSpan.FromMinutes(20); //Token有效期

                options.ValidFor = TimeSpan.FromMinutes(Convert.ToInt32(jwtAppSettingOptions[nameof(JwtIssuerOptions.ValidFor)]));

            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero



            };

            services.AddJwtBearerAuthentication(o =>
            {
                // You also need to update /wwwroot/app/scripts/app.js
                //o.Authority = Configuration["JwtIssuerOptions:authority"];

                o.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);
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
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "TwBusManagement API V2");
                c.ShowRequestHeaders();
            });

            app.UseAuthentication();

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
