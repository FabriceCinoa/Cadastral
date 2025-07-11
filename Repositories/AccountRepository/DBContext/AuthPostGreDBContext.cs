using AccountRepository.Entities;
using Common.Repository.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountRepository.DBContext
{
    public  class AuthPostGreDBContext : APostGreContext
    {
        public AuthPostGreDBContext():base()
        {
            throw new NotImplementedException("Please use Ctor with Configuration ");
        }
        public AuthPostGreDBContext(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void InitDb()
        {
            return;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          //  modelBuilder.Entity<Account>().ToTable("auth.account");
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
