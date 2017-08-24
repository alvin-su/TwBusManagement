using Microsoft.EntityFrameworkCore;
using System;
using JetBrains.Annotations;
using Tw.Bus.Entity;

namespace Tw.Bus.EntityFrameworkCore
{
    public class TwBusDbContext : DbContext
    {
        public TwBusDbContext(DbContextOptions<TwBusDbContext> options)
           : base(options)
        {

        }


        public DbSet<Usy_User> UsyUsers { get; set; }

        public DbSet<Usy_Menu> UsyMenus { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual DbSet<Usy_Role> UsyRoles { get; set; }

        /// <summary>
        /// 角色菜单
        /// </summary>
        public virtual DbSet<Usy_Role_Menu> UsyRoleMenus { get; set; }

        /// <summary>
        /// 角色用户
        /// </summary>
        public virtual DbSet<Usy_User_Role> UsyUserRoles { get; set; }

        /// <summary>
        /// 角色Action权限
        /// </summary>
        public virtual DbSet<Usy_RoleActionPermission> UsyRoleActionPermissions { get; set; }
    }
}
