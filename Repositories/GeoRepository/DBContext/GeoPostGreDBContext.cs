using NodaTime;
using Npgsql;

using Common.Repository.DBContext;
using GeoRepository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoRepository.DBContext;

internal class GeoPostGreDBContext : APostGreContext
{

    private static bool IsFirstLoad = true;
    public GeoPostGreDBContext() : base()
    {
        throw new NotImplementedException("Please use Ctor with Configuration ");
    }
    public GeoPostGreDBContext(IConfiguration configuration) : base(configuration)
    {

    }

    protected override void Configure(DbContextOptionsBuilder context)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }


    protected override void InitDb()
    {
        try
        {
            var v =this.Versions.Any() ? this.Versions.Max(c => c.Version):0;
          
            while ( v < MigrationInfos.AppVersion)
            {
                var className = $"GeoRepository.DBContext.Migration.v{v + 1}";
                try
                {
                    var IMigration = Activator.CreateInstance(Type.GetType(className)) as IDBMigation;
                    if (IMigration != null)
                    {
                        IMigration.Update(this);
                    }
                }
                catch
                {
                    continue;
                }
                finally
                {
                    v++;
                }


            }


            this.SaveChanges();



            IsFirstLoad = false;
        }
        catch (Exception ex)
        {
            return;
        }
    }

    public DbSet<CityEntity> City { get; set; }
    private DbSet<BdVersionEntity> Versions { get; set; }
}
