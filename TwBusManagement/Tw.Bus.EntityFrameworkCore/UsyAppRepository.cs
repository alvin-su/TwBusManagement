using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tw.Bus.Entity;
using Tw.Bus.IRepository;

namespace Tw.Bus.EntityFrameworkCore
{
    public class UsyAppRepository: BaseRepository<Usy_App, int>, IUsyAppRepository
    {
        public UsyAppRepository(TwBusDbContext dbContext) : base(dbContext)
        {

        }

       
    }
}
