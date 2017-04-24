using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Common;
using Tw.Bus.Entity;
using Tw.Bus.IRepository;

namespace Tw.Bus.EntityFrameworkCore
{
    // <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public abstract class BaseRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        //定义数据访问上下文对象
        protected readonly TwBusDbContext _dbContext;

        /// <summary>
        /// 通过构造函数注入得到数据上下文对象实例
        /// </summary>
        /// <param name="dbContext"></param>
        public BaseRepository(TwBusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 同步方法

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAllList()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate).ToList();
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public TEntity Get(TPrimaryKey id)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        /// <returns></returns>
        public TEntity Insert(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Add(entity);
            if (autoSave)
                Save();
            return entity;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public TEntity Update(TEntity entity, bool autoSave = true)
        {
            var obj = Get(entity.Id);
            EntityToEntity(entity, obj);
            if (autoSave)
                Save();
            return entity;
        }

        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public TEntity InsertOrUpdate(TEntity entity, bool autoSave = true)
        {
            if (Get(entity.Id) != null)
                return Update(entity, autoSave);
            return Insert(entity, autoSave);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public int Delete(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            if (autoSave)
                return Save();
            return -1;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public int Delete(TPrimaryKey id, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Remove(Get(id));
            if (autoSave)
                return Save();
            return -1;
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        public void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Where(where).ToList().ForEach(it => _dbContext.Set<TEntity>().Remove(it));
            if (autoSave)
                Save();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="startPage">页码</param>
        /// <param name="pageSize">单页数据数</param>
        /// <param name="rowCount">行数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public IQueryable<TEntity> LoadPageList(int startPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null)
        {
            var result = from p in _dbContext.Set<TEntity>()
                         select p;
            if (where != null)
                result = result.Where(where);
            if (order != null)
                result = result.OrderBy(order);
            else
                result = result.OrderBy(m => m.Id);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 事务性保存
        /// </summary>
        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        #endregion


        #region 异步方法

        public async Task<List<TEntity>> GetAllListAsync()
        {
            try
            {
                return await _dbContext.Set<TEntity>().ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Add(entity);
            if (autoSave)
            {
                await SaveAsync();
            }
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true)
        {
            var obj = Get(entity.Id);
            EntityToEntity(entity, obj);
            if (autoSave)
            {
                await SaveAsync();
            }
            return entity;
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity, bool autoSave = true)
        {
            if (Get(entity.Id) != null)
                return await UpdateAsync(entity, autoSave);
            return await InsertAsync(entity, autoSave);
        }

        public async Task<int> DeleteAsync(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            if (autoSave)
                return await SaveAsync();
            return -1;
        }

        public async Task<int> DeleteAsync(TPrimaryKey id, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Remove(Get(id));
            if (autoSave)
                return await SaveAsync();
            return -1;
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> where, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Where(where).ToList().ForEach(it => _dbContext.Set<TEntity>().Remove(it));
            if (autoSave)
                await SaveAsync();
        }

        public async Task<List<TEntity>> GetListForPageAsync(int page, int pageSize, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, Enums.OrderWay orderWay = Enums.OrderWay.ASC)
        {
            var result = from p in _dbContext.Set<TEntity>()
                         select p;
            if (filter != null)
                result = result.Where(filter);

            if (order != null)
            {
                if ((int)orderWay == 0) //升序
                {
                    result = result.OrderBy(order);
                }
                else if ((int)orderWay == 1) //将序
                {
                    result = result.OrderByDescending(order);
                }
                else
                {
                    result = result.OrderBy(order);
                }
            }
            else
            {
                if ((int)orderWay == 0) //升序
                {
                    result = result.OrderBy(m => m.Id);
                }
                else if ((int)orderWay == 1) //将序
                {
                    result = result.OrderByDescending(m => m.Id);
                }
                else
                {
                    result = result.OrderBy(m => m.Id);
                }
            }
            return await result.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var result = from p in _dbContext.Set<TEntity>()
                         select p;
            if (predicate != null)
                result = result.Where(predicate);
            return await result.CountAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        #endregion


        #region 其它方法

        private void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
        {
            foreach (var mItem in typeof(T).GetProperties())
            {
                mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
            }
        }

        /// <summary>
        /// 根据主键构建判断表达式
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
        #endregion
    }
}
