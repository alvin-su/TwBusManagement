using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tw.Bus.Common
{
    /// <summary>
    /// 使用Jwt.Net对接口传输数据进行加密
    /// </summary>
    public class JwtCryptHelper
    {
        public const string SECRETKEY = "jwttest"; //加密的密钥

        /// <summary>
        /// 使用自定义密钥加密，HS512签名
        /// </summary>
        /// <param name="strSecretKey">密钥</param>
        /// <param name="strJson">需要加密的JSON</param>
        /// <returns></returns>
        public static string EncodeByJwt(string strSecretKey, string strJson)
        {
            try
            {
                var payload = new Dictionary<string, object>
                {
                    { "Crypt", strJson }
                };

                IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var token = encoder.Encode(payload, strSecretKey);

                return token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 使用自定义的密钥解密JWT文本，HS512签名
        /// </summary>
        /// <param name="strSecretKey">密钥</param>
        /// <param name="strSecretMsg">需要解密的文本</param>
        /// <returns></returns>
        public static object DecodeByJwt(string strSecretKey, string strSecretMsg)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                //var json = decoder.Decode(strSecretMsg, strSecretKey, verify: true);
                //return json;

                var payload = decoder.DecodeToObject<IDictionary<string, object>>(strSecretMsg, strSecretKey,true);
                return payload["Crypt"];
            }
            catch (TokenExpiredException)
            {
                throw new Exception("Token has expired");

            }
            catch (SignatureVerificationException)
            {
                throw new Exception("Token has invalid signature");
            }

        }

        /// <summary>
        /// 使用默认密钥加密JSON
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static string EncodeByJwt(string strJson)
        {
            return EncodeByJwt(SECRETKEY, strJson);
        }

        /// <summary>
        /// 使用默认密钥解密文本
        /// </summary>
        /// <param name="strSecretMsg"></param>
        /// <returns></returns>
        public static object DecodeByJwt(string strSecretMsg)
        {
            return DecodeByJwt(SECRETKEY, strSecretMsg);
        }
    }
}
