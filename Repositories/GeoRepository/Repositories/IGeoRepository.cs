using GeoRepository.Entities;

namespace GeoRepository.Repositories
{
    public interface IGeoRepository
    {
        IList<CityEntity> Find(string search, int limit = 10, double precision = 0.5);
        IList<CityEntity> GetCities();
    }
}