using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tw.Bus.Cache
{
    public class MemoryCacheService:ICacheService
    {
        protected IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }


        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            object cached;
            return _cache.TryGetValue(key, out cached);
        }

        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key)
        {

            throw new NotImplementedException();

        }

        public T Get<T>(string key) where T : class
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                return _cache.Get(key) as T;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                _cache.Remove(key);

                return !Exists(key);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            try
            {
                if (keys == null)
                {
                    throw new ArgumentNullException(nameof(keys));
                }

                keys.ToList().ForEach(item => _cache.Remove(item));
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public bool Replace<T>(string key, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (Exists(key))
                    if (!Remove(key)) return false;

                return Set(key, value);
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
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (Exists(key))
                    if (!Remove(key)) return false;

                return Set(key, value, expiressAbsoulte);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> ReplaceAsync<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiressAbsoulte"></param>
        /// <returns></returns>
        public async Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _cache.Set(key, value);
                return Exists(key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Set<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _cache.Set(key, value,
                        new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(expiressAbsoulte));

                return Exists(key);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 【禁用】异步方法未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiressAbsoulte"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiressAbsoulte)
        {
            throw new NotImplementedException();
        }
    }
}
