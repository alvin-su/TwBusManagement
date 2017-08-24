using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.Web
{
    public static class SessionExtensions
    {
        public static T Get<T>(this ISession session, string key) where T : class
        {
            byte[] byteArray = null;
            if (session.TryGetValue(key, out byteArray))
            {
                using (var memoryStream = new MemoryStream(byteArray))
                {
                    var obj = ProtoBuf.Serializer.Deserialize<T>(memoryStream);
                    return obj;
                }
            }
            return null;
        }

        public static void Set<T>(this ISession session, string key, T value) where T : class
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(memoryStream, value);
                    byte[] byteArray = memoryStream.ToArray();
                    session.Set(key, byteArray);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
