using Api.Geo.Models;
using Api.Geo.Payloads;
using Bff.Search.Models;
using Common.Library.Class;
using Common.Library.DataServiceApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading.Tasks;

namespace Bff.Search.Controllers
{
    [Route("bff/[controller]")]
    [ApiController]
    public class SearchController : AuthorizedApiController
    {
        public SearchController()
        {

        }

        [HttpPost("address")]
        [AllowAnonymous]
        public async Task<SearchCityResult> Post(SearchPayload payload)
        {
            var _res = new SearchCityResult();
            try
            {

                Task<SearchResult> _apiGeoSearch = TaskHelper.PerformTaskWithTimeout<SearchResult>("ApiGeoCall", () =>
                {
                    var apicLient = new ApiClient();
                    var r = apicLient.PostAsync<SearchResult>("http://localhost:9000/search/address", payload).ConfigureAwait(false).GetAwaiter();
                    var status = r.GetResult();
                    if (status.Success && status.Data.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return status.Data;
                    }
                    return null;
                }, 1000);

                var _apiDataGouvSearch = TaskHelper.PerformTaskWithTimeout<FeatureCollectionDataGouv>("ApiGeoCall", () =>
                {
                    var apicLient = new ApiClient();
                    var r = apicLient.GetAsync<FeatureCollectionDataGouv>($"https://api-adresse.data.gouv.fr/search/?q={payload.SearchString}&limit={payload.MaxResults}").ConfigureAwait(false).GetAwaiter();
                    var status = r.GetResult();
                    if (status.Success && status.Data != null)
                    {
                        return status.Data;
                    }
                    return null;
                }, 1000);

                var res = await Task.WhenAny(_apiGeoSearch, _apiDataGouvSearch);



                if (_apiGeoSearch.Result != null && _apiGeoSearch.Result.Data?.Count() > 0)
                {
                    _res.Data = true;
                    _res.Meta.Add("Cities", _apiGeoSearch.Result.Data);
                }

                if (_apiDataGouvSearch.Result != null && _apiDataGouvSearch.Result != null)
                {
                    _res.Data = true;
                    var _datas =  _apiDataGouvSearch.Result.Features.Select( c => c.Properties).Where(c => (c.Type == "housenumber" || c.Type == "street") && c.Score >= payload.Precision).
                        Select(c => new Adress   { Name = c.Label, Position = new LatLong(c.X,c.Y)  ,  City = new City { CityName = c.City, CodeInsee = c.Citycode, Complement = c.Label, PostalCode = c.Postcode} });

                    _res.Meta.Add("Adresses", _datas);
                }
            }
            catch (Exception ex)
            {
                _res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _res.SetError(Guid.NewGuid());
                _res.Data = false;
            }

            return _res;

        }

        [HttpGet("zones")]
        [AllowAnonymous]
        public async Task<SearchZoneResult> GetZones(string codeinsee)
        {
            var _res = new SearchZoneResult();
            try
            {

                Task<SearchZoneResult> _apiGeoSearch = TaskHelper.PerformTaskWithTimeout<SearchZoneResult>("ApiGeoCall", () =>
                {
                    var apicLient = new ApiClient();
                    var r = apicLient.GetAsync<SearchZoneResult>($"http://localhost:9000/search/zones?codeinsee={codeinsee}").ConfigureAwait(false).GetAwaiter();
                    var status = r.GetResult();
                    if (status.Success && status.Data.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return status.Data;
                    }
                    return null;
                }, 1000);


                var res = await Task.WhenAny(_apiGeoSearch);


                if (_apiGeoSearch.Result != null && _apiGeoSearch.Result.Data.Features?.Count > 0)
                {
                    _res.Data = _apiGeoSearch.Result.Data;
                }

             
            }
            catch (Exception ex)
            {
                _res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _res.SetError(Guid.NewGuid());
           
            }

            return _res;

        }
    }
}
