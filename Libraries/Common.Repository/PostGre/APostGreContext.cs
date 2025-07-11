
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repository.DBContext
{

    public interface IPostGreContext
    {


    }
    public abstract class APostGreContext : DbContext, IPostGreContext
    {
        protected readonly IConfiguration Configuration;
        private static bool IsFirstLoad { get; set; } = true;

        protected APostGreContext(IConfiguration configuration) : base()
        {
            Configuration = configuration;
            if (IsFirstLoad)
            {
                this.InitDb();
            }
        }

        protected APostGreContext()
        {
            
        }

        public APostGreContext(DbContextOptions<APostGreContext> options) :
       base(options)
        {
            if (IsFirstLoad)
            {
                this.InitDb();
            }
        }


        protected abstract override void OnModelCreating(ModelBuilder modelBuilder);

        protected  override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            var connection = this.Configuration.GetConnectionString("Database");
            optionsBuilder.UseNpgsql(connection);
            Configure(optionsBuilder);



        }
          protected abstract void InitDb();
        protected  virtual void Configure(DbContextOptionsBuilder context) { }


        public IQueryable<TOut> Where<TOut>(Expression<Func<TOut, bool>> whereCondition) where TOut : class
        {
            return this.Set<TOut>().Where(whereCondition);
        }
    }
}
