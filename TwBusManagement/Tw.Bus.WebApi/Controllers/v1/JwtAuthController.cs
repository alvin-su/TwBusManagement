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
    [Produces("application/json")]
    public class JwtAuthController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;

        private readonly IUsyUserRepository _userRepository;
        /// <summary>
        /// json ���л�����, ��������ʼ���������ÿ����������ʱʹ��
        /// </summary>
        private readonly JsonSerializerSettings _seriallizerSettings;

        public JwtAuthController(IOptions<JwtIssuerOptions> jwtoptions, IUsyUserRepository userRepository)
        {
            _jwtOptions = jwtoptions.Value;
            ThrowIfInvalidOptions(_jwtOptions); //��֤jwt�����Ƿ�Ϸ�

            _seriallizerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            _userRepository = userRepository;
        }
        /// <summary>
        /// ��֤jwt�����Ƿ�Ϸ�
        /// </summary>
        /// <param name="options">���ö���</param>
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
        /// ��ȡaccecc_token�ӿ�(���뵱ǰ��¼ϵͳ���û���������);
        /// ���Ե�ʱ��Ӧ��ʹ�� 'xxx-form-urlendcode'����;
        /// ��JQuery�е��øýӿ� ��Ҫ��
        ///  xhrFields: {
        ///   withCredentials: true
        /// };
        /// ��JS ʹ��XMLHttpRequest��Ҫ��xhr.withCredentials = true ���磺
        /// var xhr = new XMLHttpRequest();
        /// xhr.withCredentials = true;
        /// </summary>
        /// <param name="applicationUser">�û���Ϣ(��¼�˺ź�����)</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/token")]
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
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssueAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("LoginCharacter")
            };

            //���� jwt ��ȫtoken, ������
            var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials
                );
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //���л����صĶ���
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
            var resJson = JsonConvert.SerializeObject(response, _seriallizerSettings);
            return new OkObjectResult(resJson);
        }

        /// <summary>
        /// ��1970�굽ĳʱ��ĺ�����
        /// </summary>
        /// <param name="date">����ʱ��</param>
        /// <returns></returns>
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// ��¼��֤����
        /// </summary>
        /// <param name="user">�û�</param>
        /// <returns>����Ȩ�޵Ķ���</returns>
        /// <remarks>�˷���Ϊ��֤����, ����ʽ��Ŀ����Ϊ��Ȩ������Ȩ��ʹ��, ע����start up�е�Ȩ�޶�Ӧ</remarks>
        private async Task<ClaimsIdentity> LoginValidate(ApplicationUser user)
        {
            Expression<Func<Usy_User, bool>> predicate = t => t.UserName == user.UserName.Trim() && t.Pwd == user.Password.Trim();

            Usy_User entityUser = await _userRepository.FirstOrDefaultAsync(predicate);

            if (entityUser != null)
            {
                return await Task.FromResult(new ClaimsIdentity(new GenericIdentity(entityUser.UserName, "Token"),
                    new[]
                    {
                        new Claim("LoginCharacter",entityUser.UserName)
                    }));
            }

            //���֤��ͨ��
            return await Task.FromResult<ClaimsIdentity>(null);
        }

    }
}