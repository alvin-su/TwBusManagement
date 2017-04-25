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
using Tw.Bus.WebApi.JwtOptions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tw.Bus.WebApi.Filters;

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


            //jwt相关配置
            //这个配置一定要加载UseMvc之前!!!
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));


            var secretKey = jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)];
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

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

                ClockSkew = TimeSpan.Zero,

            };
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
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
