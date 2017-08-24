using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Entity;

namespace Tw.Bus.IRepository
{
    public interface IUsyMenuRepository: IRepository<Usy_Menu, int>
    {
        /// <summary>
        /// 获取排序后的菜单列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="IsShowAll"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        Task<List<Usy_Menu>> GetListHaveSortAsync(int parentId, bool IsShowAll = true, List<int> roleid = null);

        /// <summary>
        /// 根据角色获取菜单
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        Task<List<int>> GetMenuIdListByRoleAsync(int roleid);

        /// <summary>
        /// 根据角色更新菜单
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="menulist"></param>
        /// <returns></returns>
        Task<int> UpdateMenuRoleByRoleIdAsync(int roleid, List<int> menulist);

        /// <summary>
        /// 删除菜单及其子菜单
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns></returns>
        Task<int> DeleteMenuWithChildAsync(int menuid);
    }
}
