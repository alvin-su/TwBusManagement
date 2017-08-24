using System;
using System.ComponentModel.DataAnnotations;

namespace Tw.Bus.Entity
{
    /// <summary>
    /// 泛型实体基类
    /// </summary>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public abstract class Entity<TPrimaryKey>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public virtual TPrimaryKey id { get; set; }
    }

    /// <summary>
    /// 定义默认主键类型为Int的实体基类
    /// </summary>
    public abstract class Entity : Entity<int>
    {

    }
}
