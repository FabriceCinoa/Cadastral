
using Common.Repository.DBContext;
using Common.Repository.PostGre;
using GeoRepository.DBContext;
using GeoRepository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GeoRepository.Repositories;

public partial  class GeoPostGreDBRepository : APostGreRepository<APostGreContext>, IGeoRepository
{


    public ZoneEntity CreateZone(ZoneEntity entity)
    {
        var z = this.Context.Set<ZoneEntity>().Where(c => c.GpuDocId == entity.GpuDocId && c.TypeZone == entity.TypeZone && c.Gid == entity.Gid).FirstOrDefault();
        if (z == null) {        
            var ret = this.Context.Add<ZoneEntity>(entity);
            if (this.SaveChanges())
                return ret.Entity;
        }

        return null;
    }

}
