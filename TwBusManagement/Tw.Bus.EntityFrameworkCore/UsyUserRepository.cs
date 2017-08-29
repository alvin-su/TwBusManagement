using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Entity;
using Tw.Bus.IRepository;

namespace Tw.Bus.EntityFrameworkCore
{
    public class UsyUserRepository : BaseRepository<Usy_User, int>, IUsyUserRepository
    {
        public UsyUserRepository(TwBusDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Usy_User> GetUserByAccountAndPwdAsync(string strAccount, string strPwd)
        {
            Usy_User model = null;
            try
            {
                model = await this.FirstOrDefaultAsync(t => t.JobNumber == strAccount && t.Pwd == strPwd.ToUpper());
                if (model != null)
                {

                    //查找角色
                    List<Usy_User_Role> lstRole = _dbContext.UsyUserRoles.Where(t => t.UserId == model.id).ToList();

                    lstRole.ForEach(t => model.LstRoleID.Add(t.RoleId.Value));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return model;
        }
    }
}
