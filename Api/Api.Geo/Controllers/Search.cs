using Api.Geo.Models;
using Api.Geo.Payloads;
using Common.Library;
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
       


        public SearchController(IConfiguration configuration, GeoPostGreDBRepository repository)
        {
            Configuration = configuration;
            Repository = repository;
        }

        public IConfiguration Configuration { get; }
        public GeoPostGreDBRepository Repository { get; }

        [HttpPost(Name = "search")]
        public SearchResult  PostSearch(SearchPayload payload)
        {
            var _res = new SearchResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            try
            {
                _res.Data = this.Repository.Find(payload.SearchString, payload.MaxResults, payload.Precision).Select(x => x.Convert<City>()).ToList();
            }
            catch (Exception ex) {
                    _res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _res.SetError(ex.Message);
            }

            return _res;
           
        }
    }
}
