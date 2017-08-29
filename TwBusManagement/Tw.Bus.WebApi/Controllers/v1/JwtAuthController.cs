using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tw.Bus.WebApi.JwtOptions;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using Tw.Bus.EntityDTO;
using Tw.Bus.IRepository;
using Tw.Bus.Entity;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Tw.Bus.WebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Tw.Bus.Common;
using Tw.Bus.WebApi.Filters;
using System.IO;

namespace Tw.Bus.WebApi.Controllers.v1
{
    //[Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/jwt")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class JwtAuthController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;

        private readonly IUsyAppRepository _appRepository;
        /// <summary>
        /// json 序列化方法, 控制器初始化后可以在每个方法返回时使用
        /// </summary>
        private readonly JsonSerializerSettings _seriallizerSettings;

        public JwtAuthController(IOptions<JwtIssuerOptions> jwtoptions, IUsyAppRepository appRepository)
        {
            _jwtOptions = jwtoptions.Value;
            ThrowIfInvalidOptions(_jwtOptions); //验证jwt配置是否合法

            _seriallizerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            _appRepository = appRepository;
        }
        /// <summary>
        /// 验证jwt配置是否合法
        /// </summary>
        /// <param name="options">配置对象</param>
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan", nameof(JwtIssuerOptions.ValidFor));
            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }


        /// <summary>
        /// 获取accecc_token接口(传入当前登录系统的用户名和密码);
        /// 测试的时候应该使用 'xxx-form-urlendcode'类型;
        /// 在JQuery中调用该接口 需要加
        ///  xhrFields: {
        ///   withCredentials: true
        /// };
        /// 纯JS 使用XMLHttpRequest需要加xhr.withCredentials = true 例如：
        /// var xhr = new XMLHttpRequest();
        /// xhr.withCredentials = true;
        /// </summary>
        /// <param name="applicationUser">用户信息(登录账号和密码)</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("token")]
        [TypeFilter(typeof(TwBusResourceFilter))]
        public async  Task<IActionResult> GetToken([FromBody]ApplicationUser applicationUser)
        {

            var identity = await LoginValidate(applicationUser);
            if (identity == null)
            {
               // log.Info($"Invalid username({applicationUser}) or password({applicationUser.Password}");

                return BadRequest("Invalid Credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.AppId),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssueAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("LoginCharacter")
            };

            //生成 jwt 安全token, 并编码
            var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials
                );
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //序列化返回的对象
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
            var resJson=  JsonHelper.SerializeObject(response);
          //  var resJson = JsonConvert.SerializeObject(response, _seriallizerSettings);
            return new OkObjectResult(resJson);
        }

        /// <summary>
        /// 从1970年到某时间的毫秒数
        /// </summary>
        /// <param name="date">计算时间</param>
        /// <returns></returns>
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// 登录验证操作
        /// </summary>
        /// <param name="appUser">用户</param>
        /// <returns>带有权限的对象</returns>
        /// <remarks>此方法为验证方法, 在正式项目中作为授权和配置权限使用, 注意与start up中的权限对应</remarks>
        private async Task<ClaimsIdentity> LoginValidate(ApplicationUser appUser)
        {
            Usy_App entityApp = await _appRepository.FirstOrDefaultAsync(t => t.AppId.Trim()== appUser.AppId.Trim() && t.AppKey == appUser.AppKey.Trim());

            if (entityApp != null)
            {
                return await Task.FromResult(new ClaimsIdentity(new GenericIdentity(entityApp.AppId, "Token"),
                    new[]
                    {
                        new Claim("LoginCharacter",entityApp.AppId)
                    }));
            }

            //身份证不通过
            return await Task.FromResult<ClaimsIdentity>(null);
        }

    }
}