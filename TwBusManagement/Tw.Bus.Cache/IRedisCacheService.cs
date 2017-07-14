using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tw.Bus.Cache
{
    public interface IRedisCacheService: ICacheService
    {

        bool KeyDelete(string key);

        bool KeyRename(string key, string newKey);

        bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 取消设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        bool KeyPersist(string key);

        /// <summary>
        /// 获取键的过期时间（键不存在返回-2，键没有过期时间返回-1）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TimeSpan KeyTtl(string key);

        long LstLPush<T>(string key, T value, When w = When.Always);

        Task<long> LstLPushAsync<T>(string key, T value, When w = When.Always);

        long LstRPush<T>(string key, T value, When w = When.Always);

        Task<long> LstRPushAsync<T>(string key, T value, When w = When.Always);

        T LstLPop<T>(string key);

        Task<T> LstLPopAsync<T>(string key);

        T LstRPop<T>(string key);

        Task<T> LstRPopAsync<T>(string key);

        long LstLLen(string key);

        Task<long> LstLLenAsync(string key);

        List<T> LstRange<T>(string key, long start = 0, long stop = -1);

        Task<List<T>> LstRangeAsync<T>(string key, long start = 0, long stop = -1);

        long LstLRem<T>(string key, T value, long count = 0);

        Task<long> LstLRemAsync<T>(string key, T value, long count = 0);

        long LstLInsertAfter<T>(string key, T pivot, T value);

        Task<long> LstLInsertAfterAsync<T>(string key, T pivot, T value);

        long LstLInsertBefore<T>(string key, T pivot, T value);

        Task<long> LstLInsertBeforeAsync<T>(string key, T pivot, T value);

        T LstRPopLPush<T>(string sourceKey, string destinationKey);

        Task<T> LstRPopLPushAsync<T>(string sourceKey, string destinationKey);

        bool LstLPushBatch<T>(string key, List<T> lstValue, When w = When.Always);

        bool LstRPushBatch<T>(string key, List<T> lstValue, When w = When.Always);
    }
}
