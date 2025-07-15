
using Common.Repository.DBContext;
using Common.Repository.PostGre;
using GeoRepository.DBContext;
using GeoRepository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GeoRepository.Repositories;

public class GeoPostGreDBRepository : APostGreRepository<APostGreContext>, IGeoRepository
{

    new GeoPostGreDBContext? Context => base.Context as GeoPostGreDBContext;
    public GeoPostGreDBRepository(IConfiguration configuration) : base(new GeoPostGreDBContext(configuration))
    {


    }

    public IList<CityEntity> GetCities()
    {
        return this.Context.City.ToList();
    }


    public IList<CityEntity> Find(string search, int limit = 10, double precision = 0.5)
    {
        string searchQ = $"{search}";
        var q = this.Context.Set<CityEntity>().Where<CityEntity>(x => EF.Functions.TrigramsSimilarity(x.Label, searchQ) > precision || EF.Functions.TrigramsSimilarity(x.CityFriendlyName, searchQ) > precision || x.PostalCode.StartsWith(search))
            .Select(x => new { x, LabelSimilarity = EF.Functions.TrigramsSimilarity(x.Label, searchQ) });

        return q.Where(d => d.LabelSimilarity > precision || d.x.PostalCode.StartsWith(search)).OrderByDescending(x => x.LabelSimilarity).Select(c => c.x).Take(limit)
         .ToList();
    }


}
