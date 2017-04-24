using System;
using System.Collections.Generic;
using System.Text;
using Tw.Bus.Entity;
using Tw.Bus.IRepository;

namespace Tw.Bus.EntityFrameworkCore
{
    public class UsyUserRepository : BaseRepository<Usy_User, int>, IUsyUserRepository
    {
        public UsyUserRepository(TwBusDbContext dbContext) : base(dbContext)
        {
        }
    }
}
