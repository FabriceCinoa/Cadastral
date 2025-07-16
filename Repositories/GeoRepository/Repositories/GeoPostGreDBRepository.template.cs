
using Common.Repository.DBContext;
using Common.Repository.PostGre;
using GeoRepository.DBContext;
using GeoRepository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GeoRepository.Repositories;

public partial  class GeoPostGreDBRepository : APostGreRepository<APostGreContext>, IGeoRepository
{




}
