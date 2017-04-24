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


        public DbSet<Usy_User> UsyUser { get; set; }


    }
}
