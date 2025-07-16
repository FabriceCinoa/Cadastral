using GeoRepository.Entities;

namespace GeoRepository.Repositories
{
    public partial interface IGeoRepository
    {

        #region Get
            IList<CityEntity> Find(string search, int limit = 10, double precision = 0.5);
            IList<CityEntity> GetCities();
        #endregion

        #region Create
        Zone CreateZone(Zone entity);
        #endregion
    }
}