using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Entity;

namespace Tw.Bus.IRepository
{
    public interface IUsyUserRepository: IRepository<Usy_User, int>
    {
        Task<Usy_User> GetUserByAccountAndPwdAsync(string strAccount, string strPwd);
    }
}
