using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Entity;
using Tw.Bus.IRepository;

namespace Tw.Bus.EntityFrameworkCore
{
    public class UsyMenuRepository: BaseRepository<Usy_Menu, int>, IUsyMenuRepository
    {
        public UsyMenuRepository(TwBusDbContext dbContext) : base(dbContext)
        {

        }
        private List<Usy_Menu> allMenu;
        public async Task<List<Usy_Menu>> GetListHaveSortAsync(int parentId, bool IsShowAll = true, List<int> roleid = null)
        {
            try
            {

                List<Usy_Menu> totallist = new List<Usy_Menu>();//用来存储数据库中所有的菜单列表
                totallist = await _dbContext.UsyMenus.ToListAsync();
                List<Usy_Menu> returnMenu = new List<Usy_Menu>();
                Usy_Menu model = new Usy_Menu();
                model = totallist.Where(c => c.Id == parentId).SingleOrDefault();
                if (model != null)
                {
                    returnMenu.Add(model);
                }

                //找出第一层父类的菜单
                allMenu = new List<Usy_Menu>();

                if (IsShowAll)//是否显示所有的
                {
                    allMenu = totallist.OrderBy(c => c.SortID).ToList();
                }
                else//按权限显示菜单
                {
                    List<int> mlist = new List<int>();//存储第一级的父菜单
                    if (roleid != null && roleid.Count != 0)
                    {
                        mlist = await _dbContext.UsyRoleMenus.Where(c => roleid.Contains(c.RoleId.Value)).Select(c => c.MenuId.Value).ToListAsync();
                    }

                    allMenu = totallist.Where(c => c.IsLock == 0 && mlist.Contains(c.Id)).OrderBy(c => c.SortID).ToList();
                }
                if (allMenu == null)
                {
                    return null;
                }
                GetChildMenu(parentId, returnMenu);


                return returnMenu;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<int>> GetMenuIdListByRoleAsync(int roleid)
        {
            try
            {
                var list = await _dbContext.UsyRoleMenus.AsNoTracking().Where(c => c.RoleId == roleid).Select(c => c.MenuId.Value).ToListAsync();
                return list;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<int> UpdateMenuRoleByRoleIdAsync(int roleid, List<int> menulist)
        {
            try
            {
                using (var tran = _dbContext.Database.BeginTransaction())
                {
                    string strsql = $"delete from usy_role_menu where roleid={ roleid }";
                   
                    int res = await _dbContext.Database.ExecuteSqlCommandAsync(strsql);
                    if (res >= 0)
                    {
                        foreach (var item in menulist)
                        {
                            Entity.Usy_Role_Menu model = new Usy_Role_Menu();
                            model.MenuId = item;
                            model.RoleId = roleid;
                            await _dbContext.UsyRoleMenus.AddAsync(model);

                        }
                        res = await _dbContext.SaveChangesAsync();
                        if (res > 0)
                        {
                            tran.Commit();
                            return res;
                        }
                    }
                    return res;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteMenuWithChildAsync(int menuid)
        {
            try
            {
                int res = 0;
                using (var tran = _dbContext.Database.BeginTransaction())
                {
                    //删除当前菜单
                    res = Delete(menuid);
                    //删除该菜单的子菜单
                    if (res > 0)
                    {
                        allMenu = GetAllList().OrderBy(c => c.SortID).ToList();
                        List<Usy_Menu> childlist = new List<Usy_Menu>();
                        GetChildMenu(menuid, childlist);//获取所有子菜单
                        if (childlist == null || childlist.Count() == 0)
                        {
                            tran.Commit();
                            return res;
                        }
                        foreach (var item in childlist)
                        {
                            var temp = Get(item.Id);
                            _dbContext.UsyMenus.Remove(temp);
                        }
                        await _dbContext.SaveChangesAsync();
                       
                        if (res > 0)
                        {
                            tran.Commit();
                        }
                    }

                }
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region 私有方法
        /// <summary>
        ///根据父ID 迭代获取子菜单
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="returnmenu"></param>

        private void GetChildMenu(int parentId, List<Usy_Menu> returnmenu)
        {
            List<Usy_Menu> templist = allMenu.Where(c => c.ParentId == parentId).ToList();
            if (templist == null || templist.Count == 0)
            {
                return;
            }

            foreach (var item in templist)
            {
                returnmenu.Add(item);
                GetChildMenu(item.Id, returnmenu);
            }
        }
        #endregion



    }
}
