using Common.Library.DataServiceApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bff.Search.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : AuthorizedApiController
    {
        public SearchController()
        {
            
        }

        [HttpGet("cities")]
        [AllowAnonymous]
        public async Task<object>  get(string s )
        {
            var apicLient = new ApiClient();
            var r = await apicLient.PostAsync<object>("http://localhost:9000/search", new { searchString = s, precision = 0.2 });
            return r.Data;
        }
    }
}
