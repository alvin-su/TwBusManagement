using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tw.Bus.Cache
{
    public class RedisCacheService: IRedisCacheService
    {
        protected IDatabase _cache;

        private ConnectionMultiplexer _connection;

        private readonly string _instance;

        public RedisCacheService(RedisCacheOptions options, int database = 0)
        {
            _connection = ConnectionMultiplexer.Connect(options.Configuration);
            _cache = _connection.GetDatabase(database);
            _instance = options.InstanceName;

        }


        public string GetKeyForRedis(string key)
        {
            return _instance + "_" + key;
        }


        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            try
            {
                return _cache.KeyDelete(GetKeyForRedis(key));
            }
            catch (Exception)
            {

                throw;
            }
        }




        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            try
            {
                return _cache.KeyRename(GetKeyForRedis(key), GetKeyForRedis(newKey));
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            try
            {
                return _cache.KeyExpire(GetKeyForRedis(key), expiry);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool KeyPersist(string key)
        {
            try
            {

                return _cache.KeyPersist(GetKeyForRedis(key));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public TimeSpan KeyTtl(string key)
        {
            try
            {
                TimeSpan ts = _cache.KeyTimeToLive(key).Value;
                return ts;

            }
            catch (Exception)
            {

                throw;
            }
        }



        #region 判断缓存是否存在
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.KeyExists(GetKeyForRedis(key));
        }

        /// <summary>
        /// 验证缓存项是否存在（异步方法）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await _cache.KeyExistsAsync(GetKeyForRedis(key));
        }
        #endregion


        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 添加缓存（异步方法）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return await _cache.StringSetAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.StringSet(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressAbsoulte);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return await _cache.StringSetAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressAbsoulte);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.KeyDelete(GetKeyForRedis(key));
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 删除缓存(异步方式)
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return await _cache.KeyDeleteAsync(GetKeyForRedis(key));
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            try
            {
                if (keys == null)
                {
                    throw new ArgumentNullException(nameof(keys));
                }

                keys.ToList().ForEach(item => Remove(item));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public T Get<T>(string key) where T : class
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var value = _cache.StringGet(GetKeyForRedis(key));

                if (!value.HasValue)
                {
                    return default(T);
                }

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var value = await _cache.StringGetAsync(GetKeyForRedis(key));

                if (!value.HasValue)
                {
                    return default(T);
                }

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Replace<T>(string key, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (Exists(key))
                    if (!Remove(key))
                        return false;

                return Set<T>(key, value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (Exists(key))
                    if (!Remove(key))
                        return false;

                return await SetAsync<T>(key, value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Replace<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (Exists(key))
                    if (!Remove(key))
                        return false;

                return Set<T>(key, value, expiressAbsoulte);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (Exists(key))
                    if (!Remove(key))
                        return false;

                return await SetAsync<T>(key, value, expiressAbsoulte);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long LstLPush<T>(string key, T value, When w = When.Always)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.ListLeftPush(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), w);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long LstRPush<T>(string key, T value, When w = When.Always)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.ListRightPush(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), w);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<long> LstLPushAsync<T>(string key, T value, When w = When.Always)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return await _cache.ListLeftPushAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), w);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<long> LstRPushAsync<T>(string key, T value, When w = When.Always)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return await _cache.ListRightPushAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), w);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T LstLPop<T>(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return JsonConvert.DeserializeObject<T>(_cache.ListLeftPop(GetKeyForRedis(key)));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> LstLPopAsync<T>(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return JsonConvert.DeserializeObject<T>(await _cache.ListLeftPopAsync(GetKeyForRedis(key)));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public T LstRPop<T>(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return JsonConvert.DeserializeObject<T>(_cache.ListRightPop(GetKeyForRedis(key)));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> LstRPopAsync<T>(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return JsonConvert.DeserializeObject<T>(await _cache.ListRightPopAsync(GetKeyForRedis(key)));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long LstLLen(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.ListLength(GetKeyForRedis(key));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<long> LstLLenAsync(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return await _cache.ListLengthAsync(GetKeyForRedis(key));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<T> LstRange<T>(string key, long start = 0, long stop = -1)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                List<T> lst = new List<T>();
                var values = _cache.ListRange(GetKeyForRedis(key), start, stop);
                foreach (RedisValue item in values)
                {
                    lst.Add(JsonConvert.DeserializeObject<T>(item));
                }
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<T>> LstRangeAsync<T>(string key, long start = 0, long stop = -1)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                List<T> lst = new List<T>();
                var values = await _cache.ListRangeAsync(GetKeyForRedis(key), start, stop);
                foreach (RedisValue item in values)
                {
                    lst.Add(JsonConvert.DeserializeObject<T>(item));
                }
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long LstLRem<T>(string key, T value, long count = 0)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _cache.ListRemove(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), count);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<long> LstLRemAsync<T>(string key, T value, long count = 0)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return await _cache.ListRemoveAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), count);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public long LstLInsertAfter<T>(string key, T pivot, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _cache.ListInsertAfter(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pivot)), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<long> LstLInsertAfterAsync<T>(string key, T pivot, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return await _cache.ListInsertAfterAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pivot)), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public long LstLInsertBefore<T>(string key, T pivot, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _cache.ListInsertBefore(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pivot)), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<long> LstLInsertBeforeAsync<T>(string key, T pivot, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return await _cache.ListInsertBeforeAsync(GetKeyForRedis(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pivot)), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public T LstRPopLPush<T>(string sourceKey, string destinationKey)
        {
            try
            {
                if (sourceKey == null)
                {
                    throw new ArgumentNullException(nameof(sourceKey));
                }
                if (destinationKey == null)
                {
                    throw new ArgumentNullException(nameof(destinationKey));
                }
                return JsonConvert.DeserializeObject<T>(_cache.ListRightPopLeftPush(GetKeyForRedis(sourceKey), GetKeyForRedis(destinationKey)));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> LstRPopLPushAsync<T>(string sourceKey, string destinationKey)
        {
            try
            {
                if (sourceKey == null)
                {
                    throw new ArgumentNullException(nameof(sourceKey));
                }
                if (destinationKey == null)
                {
                    throw new ArgumentNullException(nameof(destinationKey));
                }


                return JsonConvert.DeserializeObject<T>(await _cache.ListRightPopLeftPushAsync(GetKeyForRedis(sourceKey), GetKeyForRedis(destinationKey)));
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool LstLPushBatch<T>(string key, List<T> lstValue, When w = When.Always)
        {
            try
            {
                IBatch batch = _cache.CreateBatch();
                var tasks = new List<Task>();
                string strRedisKey = GetKeyForRedis(key);
                foreach (T value in lstValue)
                {
                    tasks.Add(batch.ListLeftPushAsync(strRedisKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), w));
                }
                batch.Execute();

                Task.WaitAll(tasks.ToArray());

                return true;
            }
            catch (Exception)
            {
                throw;
            }

            // throw new NotImplementedException();
        }

        public bool LstRPushBatch<T>(string key, List<T> lstValue, When w = When.Always)
        {
            try
            {
                IBatch batch = _cache.CreateBatch();
                var tasks = new List<Task>();
                string strRedisKey = GetKeyForRedis(key);
                foreach (T value in lstValue)
                {
                    tasks.Add(batch.ListRightPushAsync(strRedisKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), w));
                }
                batch.Execute();

                Task.WaitAll(tasks.ToArray());

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
