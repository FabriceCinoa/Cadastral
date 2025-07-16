using Api.Geo.Helpers;
using Api.Geo.Models;
using Api.Geo.Payloads;
using Common.Library;
using Common.Library.DataServiceApi;
using Common.Repository.Interfaces;
using GeoRepository.Entities;
using GeoRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {

        public IConfiguration Configuration { get; }
        public IGeoRepository Repository { get; }

        public SearchController(IConfiguration configuration, IGeoRepository repository)
        {
            Configuration = configuration;
            Repository = repository;
        }



        [HttpPost("address")]
        public SearchResult PostSearch(SearchPayload payload)
        {
            var _res = new SearchResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            try
            {
                _res.Data = this.Repository.Find(payload.SearchString, payload.MaxResults, payload.Precision).Select(x => x.Convert<City>()).ToList();
            }
            catch (Exception ex)
            {
                _res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _res.SetError(ex.Message);
            }

            return _res;

        }


        [HttpGet("zones")]
        public SearchZoneResult GetZones(string codeinsee)
        {
            var _res = new SearchZoneResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            try
            {
                var _datas = this.Repository.GetZonesBy(codeinsee);
                if (_datas?.Any() ?? false)
                {
                    var _features = _datas.Select(c =>
                    new Feature()
                    {
                        Properties = new Dictionary<string, object>() {
                           { "x",c.Geometry.Centroid.X},
                           { "y", c.Geometry.Centroid.Y } ,
                           { "surface", c.Geometry.GetSurfaceMeterPerSquare()},
                        //    {"zone",c.ZoneCode },
                            {"type",c.TypeZone },
                       //     {"ud",c.Gid },
                            {"docId",c.GpuDocId},
                            {"libelle",c.Libelong },
                            {"uid",c.Uid }
                        },
                        Geometry = c.Geometry.GetGeometry(),
                        Type = "Feature"
                    }
                    );
                    _res.Data = (new FeatureCollection() { Type = "FeatureCollection", Features = _features.ToList() });
                }



            }
            catch (Exception ex)
            {
                _res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _res.SetError(ex.Message);
            }

            return _res;
        }


    }
}
