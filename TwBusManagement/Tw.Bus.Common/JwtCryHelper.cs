using Jose;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Tw.Bus.Common
{
    /// <summary>
    /// 使用JWT对接口传输数据进行加密
    /// </summary>
    public class JwtCryHelper
    {
        public const string SECRETKEY = "jwttest"; //加密的密钥

        #region
        /// <summary>
        /// 使用自定义密钥加密，HS512签名
        /// </summary>
        /// <param name="strSecretKey">密钥</param>
        /// <param name="strPayload">需要加密的对象</param>
        /// <returns></returns>
        public static string EncodeBYJWT(string strSecretKey, object strPayload)
        {
            try
            {
                byte[] key = Encoding.ASCII.GetBytes(strSecretKey);
                return Jose.JWT.Encode(strPayload, key, Jose.JwsAlgorithm.HS512);
            }
            catch (Exception)
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
        public static object DecodeByJWT(string strSecretKey, string strSecretMsg)
        {

            byte[] key = Encoding.ASCII.GetBytes(strSecretKey);

            return Jose.JWT.Decode(strSecretMsg, key, Jose.JwsAlgorithm.HS512);
        }

        /// <summary>
        /// 使用默认密钥加密对象
        /// </summary>
        /// <param name="strPayload"></param>
        /// <returns></returns>
        public static string EncodeBYJWT(object strPayload)
        {
            return EncodeBYJWT(SECRETKEY, strPayload);
        }

        /// <summary>
        /// 使用默认密钥解密文本
        /// </summary>
        /// <param name="strSecretMsg"></param>
        /// <returns></returns>
        public static object DecodeByJWT(string strSecretMsg)
        {
            return DecodeByJWT(SECRETKEY, strSecretMsg);
        }
        #endregion

        #region  RS
        public static string EncodeBYJWTRS(string strSecretKey, object strPayload)
        {
            //byte[] key = Encoding.ASCII.GetBytes(strSecretKey);
            //return Jose.JWT.Encode(strPayload, key, Jose.JwsAlgorithm.RS512);

            var privateKey = new X509Certificate2("my-key.p12", strSecretKey, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet).GetRSAPrivateKey() as RSACng;

            string token = Jose.JWT.Encode(strPayload, privateKey, JwsAlgorithm.RS256);
            return token;
        }

        public static object DecodeByJWTRS(string strSecretKey, string strSecretMsg)
        {

            var privateKey = new X509Certificate2("my-key.p12", strSecretKey, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet).GetRSAPrivateKey() as RSACng;

            return Jose.JWT.Decode(strSecretMsg, privateKey, Jose.JwsAlgorithm.RS512);
        }


        #endregion

    }
}
